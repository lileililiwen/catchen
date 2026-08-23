import 'package:catchen_consumer/main.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('renders the shell home screen', (tester) async {
    await tester.pumpWidget(const MainApp());

    expect(find.text('Hello World!'), findsOneWidget);
  });
}
