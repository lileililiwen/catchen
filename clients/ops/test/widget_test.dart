import 'package:catchen_ops/main.dart' as app;
import 'package:catchen_ops/session.dart';
import 'package:flutter_test/flutter_test.dart';

const _adminToken =
    'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiI0NTY3OCIsImVtYWlsIjoiYWRtaW5AY2F0Y2hlbi5s'
    'b2NhbCIsInJvbGUiOiJBZG1pbmlzdHJhdG9yIn0.ignored';
const _userToken =
    'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0IiwiZW1haWwiOiJ1c2VyQGV4YW1wbGUuY29t'
    'Iiwicm9sZSI6IlJlZ3VsYXJVc2VyIn0.ignored';

void main() {
  testWidgets('boots through main() and shows the sign-in gate', (
    tester,
  ) async {
    Session.current = null;
    app.main();
    await tester.pump();

    expect(find.text('Catchen Ops — Sign in'), findsOneWidget);
  });

  testWidgets('administrators reach the channel approvals console', (
    tester,
  ) async {
    Session.current = Session.fromToken(_adminToken);
    app.main();
    await tester.pump();

    expect(find.text('Channel approvals'), findsOneWidget);
  });

  testWidgets('regular users are denied the operations console', (
    tester,
  ) async {
    Session.current = Session.fromToken(_userToken);
    app.main();
    await tester.pump();

    expect(find.textContaining('restricted to administrators'), findsOneWidget);
  });

  test('session decoding rejects malformed tokens', () {
    expect(Session.fromToken('not-a-jwt'), isNull);
  });
}
