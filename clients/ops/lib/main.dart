import 'package:catchen_api_client/api.dart';
import 'package:flutter/material.dart';

import 'session.dart';

const _kApiBaseDefault = 'http://localhost:8080';
const kApiBaseUrl = String.fromEnvironment(
  'CATCHEN_API_BASE',
  defaultValue: _kApiBaseDefault,
);

CatchenApiApi createApi({String? bearerToken}) => CatchenApiApi(
  ApiClient(
    basePath: kApiBaseUrl,
    authentication: HttpBearerAuth()..accessToken = bearerToken,
  ),
);

Future<void> main() async {
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(home: AuthGate());
  }
}

/// Role-aware gate: only administrators reach the operations console;
/// regular users get an explicit access-denied screen.
class AuthGate extends StatefulWidget {
  const AuthGate({super.key});

  @override
  State<AuthGate> createState() => _AuthGateState();
}

class _AuthGateState extends State<AuthGate> {
  @override
  Widget build(BuildContext context) {
    final session = Session.current;
    if (session is! Session) {
      return const LoginScreen();
    }
    return session.isAdministrator
        ? ChannelsScreen(session: session)
        : const AccessDeniedScreen();
  }
}

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _email = TextEditingController();
  final _password = TextEditingController();
  String? _error;
  bool _busy = false;

  Future<void> _submit() async {
    setState(() {
      _busy = true;
      _error = null;
    });

    try {
      final response = await createApi().apiAuthLoginPost(
        LoginEndpointRequest(
          email: _email.text.trim(),
          password: _password.text,
        ),
      );

      final token = response?.token;
      final session = token is String ? Session.fromToken(token) : null;
      if (!mounted) {
        return;
      }

      if (session is! Session) {
        setState(() {
          _error = 'Sign-in failed. Check your credentials.';
          _busy = false;
        });
        return;
      }

      Session.current = session;
      setState(() => _busy = false);
      Navigator.of(context).pushReplacement(
        MaterialPageRoute<void>(
          builder: (_) => session.isAdministrator
              ? ChannelsScreen(session: session)
              : const AccessDeniedScreen(),
        ),
      );
    } catch (error) {
      if (!mounted) {
        return;
      }
      setState(() {
        _error = 'Could not reach the service. Try again later.';
        _busy = false;
      });
    }
  }

  @override
  void dispose() {
    _email.dispose();
    _password.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Catchen Ops — Sign in')),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            TextField(
              controller: _email,
              decoration: const InputDecoration(labelText: 'Operator email'),
              keyboardType: TextInputType.emailAddress,
            ),
            TextField(
              controller: _password,
              decoration: const InputDecoration(labelText: 'Password'),
              obscureText: true,
            ),
            const SizedBox(height: 16),
            if (_error is String)
              Text(_error!, style: const TextStyle(color: Colors.red)),
            FilledButton(
              onPressed: _busy ? null : _submit,
              child: Text(_busy ? 'Signing in…' : 'Sign in'),
            ),
          ],
        ),
      ),
    );
  }
}

class AccessDeniedScreen extends StatelessWidget {
  const AccessDeniedScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Catchen Ops')),
      body: const Center(
        child: Text('This console is restricted to administrators.'),
      ),
    );
  }
}

class ChannelsScreen extends StatefulWidget {
  const ChannelsScreen({required this.session, super.key});

  final Session session;

  @override
  State<ChannelsScreen> createState() => _ChannelsScreenState();
}

class _ChannelsScreenState extends State<ChannelsScreen> {
  List<ApprovedChannel>? _approvals;
  String? _message;

  @override
  void initState() {
    super.initState();
    _reload();
  }

  Future<void> _reload() async {
    try {
      final response = await createApi(bearerToken: widget.session.token)
          .apiAdminPromotionChannelsApprovalsGet();
      if (!mounted) {
        return;
      }
      setState(() {
        _approvals = response?.approvals?.toList();
        _message = null;
      });
    } catch (error) {
      if (!mounted) {
        return;
      }
      setState(() => _message = 'Could not load approvals.');
    }
  }

  Future<void> _approve(String channel, String kind) async {
    try {
      await createApi(bearerToken: widget.session.token)
          .apiAdminPromotionChannelsApprovalsPost(
            ApproveChannelRequest(channel: channel.trim(), kind: kind),
          );
      await _reload();
    } on ApiException catch (exception) {
      if (!mounted) {
        return;
      }
      final violation =
          exception.message?.contains('channel_prohibited') == true
          ? 'Domestic channels can never be approved.'
          : exception.message;
      setState(() => _message = violation);
    } catch (error) {
      if (!mounted) {
        return;
      }
      setState(() => _message = 'Approval request failed.');
    }
  }

  @override
  Widget build(BuildContext context) {
    final approvals = _approvals;
    return Scaffold(
      appBar: AppBar(
        title: const Text('Channel approvals'),
        actions: [
          IconButton(
            tooltip: 'Sign out',
            icon: const Icon(Icons.logout),
            onPressed: () {
              Session.signOut();
              Navigator.of(context).pushReplacement(
                MaterialPageRoute<void>(builder: (_) => const AuthGate()),
              );
            },
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _showApproveDialog(context),
        label: const Text('Approve channel'),
        icon: const Icon(Icons.add),
      ),
      body: Column(
        children: [
          if (_message is String)
            Padding(
              padding: const EdgeInsets.all(12),
              child: Text(_message!, style: const TextStyle(color: Colors.red)),
            ),
          Expanded(
            child: approvals is List<ApprovedChannel>
                ? ListView(
                    children: [
                      for (final approval in approvals)
                        ListTile(
                          title: Text(approval.channel ?? ''),
                          subtitle: Text(approval.kind ?? ''),
                          trailing: Text(
                            (approval.approvedAtUtc ?? DateTime.now())
                                .toLocal()
                                .toString()
                                .split('.')
                                .first,
                          ),
                        ),
                    ],
                  )
                : const Center(child: CircularProgressIndicator()),
          ),
        ],
      ),
    );
  }

  Future<void> _showApproveDialog(BuildContext context) async {
    final channelController = TextEditingController();
    var kind = 'promotion';

    await showDialog<void>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Approve campaign channel'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: channelController,
              decoration: const InputDecoration(
                labelText: 'Channel slug (e.g. google_ads)',
              ),
            ),
            DropdownButtonFormField<String>(
              initialValue: kind,
              items: const [
                DropdownMenuItem(value: 'promotion', child: Text('Promotion')),
                DropdownMenuItem(
                  value: 'distribution',
                  child: Text('Distribution'),
                ),
              ],
              onChanged: (value) => kind = value ?? 'promotion',
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(dialogContext),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () {
              Navigator.pop(dialogContext);
              if (channelController.text.trim().isNotEmpty) {
                _approve(channelController.text, kind);
              }
            },
            child: const Text('Approve'),
          ),
        ],
      ),
    );
  }
}
