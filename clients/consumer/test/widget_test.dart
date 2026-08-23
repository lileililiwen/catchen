import 'package:catchen_consumer/main.dart' as app;
import 'package:catchen_consumer/session.dart';
import 'package:flutter_test/flutter_test.dart';

// A structurally valid (unsigned) token used only to exercise claim decoding.
const _fakeToken =
    'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0IiwiZW1haWwiOiJ1c2VyQGV4YW1wbGUuY29t'
    'Iiwicm9sZSI6IlJlZ3VsYXJVc2VyIn0.ignored';

void main() {
  testWidgets('boots through main() and shows the sign-in gate', (
    tester,
  ) async {
    Session.current = null;
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

  test('session decoding rejects malformed tokens', () {
    expect(Session.fromToken('not-a-jwt'), isNull);
  });
}
