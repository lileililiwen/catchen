import 'package:catchen_api_client/api.dart';
import 'package:catchen_consumer/main.dart' as app;
import 'package:catchen_consumer/session.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';

// A structurally valid (unsigned) token used only to exercise claim decoding.
const _fakeToken =
    'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0IiwiZW1haWwiOiJ1c2VyQGV4YW1wbGUuY29t'
    'Iiwicm9sZSI6IlJlZ3VsYXJVc2VyIn0.ignored';

class _MockApi extends Mock implements CatchenApiApi {}

void main() {
  setUp(() {
    Session.current = null;
    registerFallbackValue(LoginEndpointRequest(email: 'x', password: 'y'));
  });

  testWidgets('boots through main() and shows the sign-in gate', (
    tester,
  ) async {
    app.main();
    await tester.pump();

    expect(find.text('Catchen — Sign in'), findsOneWidget);
  });

  testWidgets('a signed-in consumer lands on the home screen', (tester) async {
    Session.current = Session.fromToken(_fakeToken);
    app.main();
    await tester.pump();

    expect(find.textContaining('Welcome'), findsOneWidget);
    expect(find.text('Role: RegularUser'), findsOneWidget);
  });

  testWidgets('successful sign-in navigates to the home screen', (
    tester,
  ) async {
    final api = _MockApi();
    when(() => api.apiAuthLoginPost(any())).thenAnswer(
      (_) async =>
          LoginResponse(token: _fakeToken, expiresAtUtc: DateTime.utc(2026)),
    );

    await tester.pumpWidget(app.MainApp(loginApi: api));
    await tester.pump();

    await tester.enterText(find.byType(TextField).at(0), 'user@example.com');
    await tester.enterText(find.byType(TextField).at(1), 'Passw0rd!long');
    await tester.tap(find.text('Sign in'));
    await tester.pumpAndSettle();

    expect(find.textContaining('Welcome'), findsOneWidget);
    verify(() => api.apiAuthLoginPost(any())).called(1);
  });

  testWidgets('failed sign-in surfaces an inline error', (tester) async {
    final api = _MockApi();
    when(
      () => api.apiAuthLoginPost(any()),
    ).thenAnswer((_) async => LoginResponse(token: null, expiresAtUtc: null));

    await tester.pumpWidget(app.MainApp(loginApi: api));
    await tester.pump();
    await tester.tap(find.text('Sign in'));
    await tester.pump();

    expect(
      find.text('Sign-in failed. Check your credentials.'),
      findsOneWidget,
    );
  });

  testWidgets('unreachable service shows a retry hint', (tester) async {
    final api = _MockApi();
    when(() => api.apiAuthLoginPost(any())).thenThrow(Exception('offline'));

    await tester.pumpWidget(app.MainApp(loginApi: api));
    await tester.pump();
    await tester.tap(find.text('Sign in'));
    await tester.pump();

    expect(
      find.text('Could not reach the service. Try again later.'),
      findsOneWidget,
    );
  });

  testWidgets('sign-out returns to the gate', (tester) async {
    Session.current = Session.fromToken(_fakeToken);
    app.main();
    await tester.pump();

    await tester.tap(find.byTooltip('Sign out'));
    await tester.pumpAndSettle();

    expect(find.text('Catchen — Sign in'), findsOneWidget);
  });

  test('session decoding rejects malformed tokens', () {
    expect(Session.fromToken('not-a-jwt'), isNull);
  });
}
