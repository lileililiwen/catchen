import 'package:catchen_api_client/api.dart';
import 'package:catchen_ops/main.dart' as app;
import 'package:catchen_ops/session.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';

const _adminToken =
    'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiI0NTY3OCIsImVtYWlsIjoiYWRtaW5AY2F0Y2hlbi5s'
    'b2NhbCIsInJvbGUiOiJBZG1pbmlzdHJhdG9yIn0.ignored';
const _userToken =
    'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0IiwiZW1haWwiOiJ1c2VyQGV4YW1wbGUuY29t'
    'Iiwicm9sZSI6IlJlZ3VsYXJVc2VyIn0.ignored';

class _MockApi extends Mock implements CatchenApiApi {}

void main() {
  setUp(() {
    Session.current = null;
    registerFallbackValue(LoginEndpointRequest(email: 'x', password: 'y'));
    registerFallbackValue(
      ApproveChannelRequest(channel: 'x', kind: 'promotion'),
    );
  });

  testWidgets('boots through main() and shows the sign-in gate', (
    tester,
  ) async {
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

  testWidgets('successful operator sign-in opens the console', (tester) async {
    final loginApi = _MockApi();
    when(() => loginApi.apiAuthLoginPost(any())).thenAnswer(
      (_) async =>
          LoginResponse(token: _adminToken, expiresAtUtc: DateTime.utc(2026)),
    );
    final channelsApi = _MockApi();
    when(() => channelsApi.apiAdminPromotionChannelsApprovalsGet())
        .thenAnswer((_) async => ApprovedChannelsResponse(approvals: const []));

    await tester.pumpWidget(
      app.MainApp(loginApi: loginApi, channelsApi: channelsApi),
    );
    await tester.pump();
    await tester.enterText(find.byType(TextField).at(0), 'admin@catchen.local');
    await tester.enterText(find.byType(TextField).at(1), 'Admin#Local-2026');
    await tester.tap(find.text('Sign in'));
    await tester.pumpAndSettle();

    expect(find.text('Channel approvals'), findsOneWidget);
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

  testWidgets('approvals list renders approved channels', (tester) async {
    final channelsApi = _MockApi();
    when(() => channelsApi.apiAdminPromotionChannelsApprovalsGet()).thenAnswer(
      (_) async => ApprovedChannelsResponse(
        approvals: [
          ApprovedChannel(
            channel: 'google_ads',
            kind: 'promotion',
            approvedByUserId: '00000000-0000-0000-0000-000000000001',
            approvedAtUtc: DateTime.utc(2026, 8, 1),
          ),
          ApprovedChannel(
            channel: 'instagram',
            kind: 'distribution',
            approvedByUserId: '00000000-0000-0000-0000-000000000002',
            approvedAtUtc: DateTime.utc(2026, 8, 2),
          ),
        ],
      ),
    );
    final loginApi = _MockApi();
    when(() => loginApi.apiAuthLoginPost(any())).thenAnswer(
      (_) async =>
          LoginResponse(token: _adminToken, expiresAtUtc: DateTime.utc(2026)),
    );

    await tester.pumpWidget(
      app.MainApp(loginApi: loginApi, channelsApi: channelsApi),
    );
    await tester.pump();
    await tester.enterText(find.byType(TextField).at(0), 'admin@catchen.local');
    await tester.enterText(find.byType(TextField).at(1), 'Admin#Local-2026');
    await tester.tap(find.text('Sign in'));
    await tester.pumpAndSettle();

    expect(find.text('google_ads'), findsOneWidget);
    expect(find.text('instagram'), findsOneWidget);
  });

  testWidgets('approving a channel posts and refreshes', (tester) async {
    final channelsApi = _MockApi();
    when(() => channelsApi.apiAdminPromotionChannelsApprovalsGet())
        .thenAnswer((_) async => ApprovedChannelsResponse(approvals: const []));
    when(() => channelsApi.apiAdminPromotionChannelsApprovalsPost(any()))
        .thenAnswer((_) async => ApprovalResponse(id: 'approval-1'));
    final loginApi = _MockApi();
    when(() => loginApi.apiAuthLoginPost(any())).thenAnswer(
      (_) async =>
          LoginResponse(token: _adminToken, expiresAtUtc: DateTime.utc(2026)),
    );

    await tester.pumpWidget(
      app.MainApp(loginApi: loginApi, channelsApi: channelsApi),
    );
    await tester.pump();
    await tester.enterText(find.byType(TextField).at(0), 'admin@catchen.local');
    await tester.enterText(find.byType(TextField).at(1), 'Admin#Local-2026');
    await tester.tap(find.text('Sign in'));
    await tester.pumpAndSettle();

    await tester.tap(find.byType(FloatingActionButton));
    await tester.pumpAndSettle();
    expect(find.text('Approve campaign channel'), findsOneWidget);
    expect(find.byType(TextField), findsOneWidget);
    await tester.enterText(find.byType(TextField), 'meta_ads');
    expect(find.text('meta_ads'), findsOneWidget);
    await tester.tap(find.text('Approve'));
    await tester.pumpAndSettle();

    verify(
      () => channelsApi.apiAdminPromotionChannelsApprovalsPost(
        ApproveChannelRequest(channel: 'meta_ads', kind: 'promotion'),
      ),
    ).called(1);
  });

  testWidgets('prohibited channels show the policy message', (tester) async {
    final channelsApi = _MockApi();
    when(() => channelsApi.apiAdminPromotionChannelsApprovalsGet())
        .thenAnswer((_) async => ApprovedChannelsResponse(approvals: const []));
    when(() => channelsApi.apiAdminPromotionChannelsApprovalsPost(any()))
        .thenThrow(ApiException(422, 'channel_prohibited'));
    final loginApi = _MockApi();
    when(() => loginApi.apiAuthLoginPost(any())).thenAnswer(
      (_) async =>
          LoginResponse(token: _adminToken, expiresAtUtc: DateTime.utc(2026)),
    );

    await tester.pumpWidget(
      app.MainApp(loginApi: loginApi, channelsApi: channelsApi),
    );
    await tester.pump();
    await tester.enterText(find.byType(TextField).at(0), 'admin@catchen.local');
    await tester.enterText(find.byType(TextField).at(1), 'Admin#Local-2026');
    await tester.tap(find.text('Sign in'));
    await tester.pumpAndSettle();

    await tester.tap(find.byType(FloatingActionButton));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextField), 'xiaohongshu');
    await tester.tap(find.text('Approve'));
    await tester.pumpAndSettle();

    expect(
      find.textContaining('Domestic channels can never be approved'),
      findsOneWidget,
    );
  });

  testWidgets('sign-out from the console returns to the gate', (tester) async {
    final channelsApi = _MockApi();
    when(() => channelsApi.apiAdminPromotionChannelsApprovalsGet())
        .thenAnswer((_) async => ApprovedChannelsResponse(approvals: const []));

    Session.current = Session.fromToken(_adminToken);
    await tester.pumpWidget(app.MainApp(channelsApi: channelsApi));
    await tester.pumpAndSettle();

    await tester.tap(find.byTooltip('Sign out'));
    await tester.pumpAndSettle();

    expect(find.text('Catchen Ops — Sign in'), findsOneWidget);
  });

  test('session decoding rejects malformed tokens', () {
    expect(Session.fromToken('not-a-jwt'), isNull);
  });
}
