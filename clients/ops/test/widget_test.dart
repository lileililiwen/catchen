import 'package:catchen_ops/main.dart' as app;
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('boots through main() and renders the shell home screen', (
    tester,
  ) async {
    app.main();
    await tester.pump();

    expect(find.text('Hello World!'), findsOneWidget);
  });
}
