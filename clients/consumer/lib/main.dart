import 'package:catchen_api_client/api.dart';
import 'package:flutter/material.dart';

import 'session.dart';

/// Base URL of the Catchen API. Override with:
/// `flutter run --dart-define=CATCHEN_API_BASE=http://host:port`
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

void main() {
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(home: AuthGate());
  }
}

/// Role-aware gate: signed-in consumers land on the home screen; everyone
/// else sees the login form.
class AuthGate extends StatefulWidget {
  const AuthGate({super.key});

  @override
  State<AuthGate> createState() => _AuthGateState();
}

class _AuthGateState extends State<AuthGate> {
  @override
  Widget build(BuildContext context) {
    final session = Session.current;
    return session is Session
        ? HomeScreen(session: session)
        : const LoginScreen();
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
        MaterialPageRoute<void>(builder: (_) => HomeScreen(session: session)),
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
      appBar: AppBar(title: const Text('Catchen — Sign in')),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            TextField(
              controller: _email,
              decoration: const InputDecoration(labelText: 'Email'),
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

class HomeScreen extends StatelessWidget {
  const HomeScreen({required this.session, super.key});

  final Session session;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Catchen'),
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
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text('Welcome, ${session.email}'),
            const SizedBox(height: 8),
            Text('Role: ${session.role}'),
          ],
        ),
      ),
    );
  }
}
