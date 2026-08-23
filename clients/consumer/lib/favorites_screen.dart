import 'package:catchen_api_client/api.dart';
import 'package:flutter/material.dart';

import 'session.dart';

/// The signed-in consumer's saved recipes (task 2.4: favorites persist
/// across authenticated devices).
class FavoritesScreen extends StatefulWidget {
  const FavoritesScreen({required this.api, required this.session, super.key});

  final CatchenApiApi api;
  final Session session;

  @override
  State<FavoritesScreen> createState() => _FavoritesScreenState();
}

class _FavoritesScreenState extends State<FavoritesScreen> {
  List<CatalogSummary>? _items;
  String? _error;

  @override
  void initState() {
    super.initState();
    _reload();
  }

  Future<void> _reload() async {
    try {
      final response = await widget.api.apiCatalogFavoritesGet();
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
      setState(() => _error = 'Could not load favorites.');
    }
  }

  @override
  Widget build(BuildContext context) {
    final items = _items;
    if (_error is String) {
      return Center(child: Text(_error!));
    }
    if (items is! List<CatalogSummary>) {
      return const Center(child: CircularProgressIndicator());
    }
    if (items.isEmpty) {
      return const Center(
        child: Text('No favorites yet — browse recipes to add some.'),
      );
    }
    return ListView(
      children: [
        for (final recipe in items)
          ListTile(
            leading: const Icon(Icons.favorite),
            title: Text(recipe.title ?? ''),
            subtitle: Text(recipe.cuisine ?? ''),
          ),
      ],
    );
  }
}
