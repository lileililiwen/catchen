import 'package:catchen_api_client/api.dart';
import 'package:flutter/material.dart';

import 'favorites_screen.dart';
import 'recipe_detail_screen.dart';
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
  const MainApp({super.key, this.loginApi, this.catalogApi});

  final CatchenApiApi? loginApi;
  final CatchenApiApi? catalogApi;

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: AuthGate(loginApi: loginApi, catalogApi: catalogApi),
    );
  }
}

class AuthGate extends StatefulWidget {
  const AuthGate({super.key, this.loginApi, this.catalogApi});

  final CatchenApiApi? loginApi;
  final CatchenApiApi? catalogApi;

  @override
  State<AuthGate> createState() => _AuthGateState();
}

class _AuthGateState extends State<AuthGate> {
  @override
  Widget build(BuildContext context) {
    final session = Session.current;
    if (session is! Session) {
      return LoginScreen(
        api: widget.loginApi,
        onSignedIn: (session) => setState(() => Session.current = session),
      );
    }
    return HomeShell(session: session, api: widget.catalogApi);
  }
}

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key, this.api, required this.onSignedIn});

  final CatchenApiApi? api;
  final void Function(Session session) onSignedIn;

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
      final api = widget.api ?? createApi();
      final response = await api.apiAuthLoginPost(
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

      widget.onSignedIn(session);
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

/// Signed-in shell: catalog browsing plus a favorites tab.
class HomeShell extends StatefulWidget {
  const HomeShell({required this.session, super.key, this.api});

  final Session session;
  final CatchenApiApi? api;

  @override
  State<HomeShell> createState() => _HomeShellState();
}

class _HomeShellState extends State<HomeShell> {
  var _tab = 0;

  CatchenApiApi get _api =>
      widget.api ?? createApi(bearerToken: widget.session.token);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_tab == 0 ? 'Catchen Recipes' : 'My Favorites'),
        actions: [
          IconButton(
            tooltip: 'Sign out',
            icon: const Icon(Icons.logout),
            onPressed: () {
              Session.signOut();
              setState(() {});
            },
          ),
        ],
      ),
      body: _tab == 0
          ? CatalogScreen(api: _api, session: widget.session)
          : FavoritesScreen(api: _api, session: widget.session),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _tab,
        onDestinationSelected: (index) => setState(() => _tab = index),
        destinations: const [
          NavigationDestination(icon: Icon(Icons.restaurant), label: 'Recipes'),
          NavigationDestination(icon: Icon(Icons.favorite), label: 'Favorites'),
        ],
      ),
    );
  }
}

/// Browse published recipes with combined filters (task 2.4).
class CatalogScreen extends StatefulWidget {
  const CatalogScreen({required this.api, required this.session, super.key});

  final CatchenApiApi api;
  final Session session;

  @override
  State<CatalogScreen> createState() => _CatalogScreenState();
}

class _CatalogScreenState extends State<CatalogScreen> {
  List<CatalogSummary>? _items;
  String? _search;
  String? _category;
  String? _difficulty;
  String? _error;

  static const _categories = [
    ('sichuan', 'Sichuan'),
    ('cantonese', 'Cantonese'),
    ('flour_based', 'Flour-based'),
    ('vegetarian', 'Vegetarian'),
    ('quick_home_style', 'Quick home-style'),
  ];

  static const _difficulties = [
    ('easy', 'Easy'),
    ('medium', 'Medium'),
    ('hard', 'Hard'),
  ];

  @override
  void initState() {
    super.initState();
    _reload();
  }

  Future<void> _reload() async {
    try {
      final response = await widget.api.apiCatalogRecipesGet(
        category: _category,
        difficulty: _difficulty,
        ingredient: null,
        q: _search,
      );
      if (!mounted) {
        return;
      }
      setState(() {
        _items = response?.items?.toList();
        _error = null;
      });
    } catch (error) {
      if (!mounted) {
        return;
      }
      setState(() => _error = 'Could not load recipes.');
    }
  }

  @override
  Widget build(BuildContext context) {
    final items = _items;
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(12),
          child: TextField(
            decoration: const InputDecoration(
              labelText: 'Search recipes',
              prefixIcon: Icon(Icons.search),
              border: OutlineInputBorder(),
            ),
            onSubmitted: (value) {
              _search = value;
              _reload();
            },
          ),
        ),
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          padding: const EdgeInsets.symmetric(horizontal: 12),
          child: Row(
            children: [
              DropdownButton<String>(
                hint: const Text('Cuisine'),
                value: _category,
                items: [
                  for (final (value, label) in _categories)
                    DropdownMenuItem(value: value, child: Text(label)),
                ],
                onChanged: (value) {
                  setState(() => _category = value);
                  _reload();
                },
              ),
              const SizedBox(width: 12),
              DropdownButton<String>(
                hint: const Text('Difficulty'),
                value: _difficulty,
                items: [
                  for (final (value, label) in _difficulties)
                    DropdownMenuItem(value: value, child: Text(label)),
                ],
                onChanged: (value) {
                  setState(() => _difficulty = value);
                  _reload();
                },
              ),
            ],
          ),
        ),
        if (_error is String)
          Padding(
            padding: const EdgeInsets.all(12),
            child: Text(_error!, style: const TextStyle(color: Colors.red)),
          ),
        Expanded(
          child: items is List<CatalogSummary>
              ? ListView.builder(
                  itemCount: items.length,
                  itemBuilder: (context, index) {
                    final recipe = items[index];
                    return ListTile(
                      title: Text(recipe.title ?? ''),
                      subtitle: Text(
                        '${recipe.cuisine ?? ''} · ${recipe.difficulty ?? ''}'
                        '${(recipe.isFree ?? false) ? '' : ' · Premium'}',
                      ),
                      trailing: const Icon(Icons.chevron_right),
                      onTap: () => Navigator.of(context).push(
                        MaterialPageRoute<void>(
                          builder: (_) => RecipeDetailScreen(
                            api: widget.api,
                            session: widget.session,
                            recipeId: recipe.recipeId ?? '',
                          ),
                        ),
                      ),
                    );
                  },
                )
              : const Center(child: CircularProgressIndicator()),
        ),
      ],
    );
  }
}
