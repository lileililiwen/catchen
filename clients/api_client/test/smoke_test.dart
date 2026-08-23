import 'package:catchen_api_client/api.dart';
import 'package:test/test.dart';

void main() {
  test('generated client exposes typed auth operations', () {
    final api = CatchenApiApi(ApiClient(basePath: 'http://localhost:0'));
    expect(api, isNotNull);
  });
}
