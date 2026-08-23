import 'dart:convert';

import 'package:catchen_api_client/api.dart';
import 'package:flutter/material.dart';

import 'session.dart';

/// Entitlement-aware recipe detail (task 2.4): free or entitled users see the
/// full validated content; locked premium recipes show preview + purchase
/// options only.
class RecipeDetailScreen extends StatefulWidget {
  const RecipeDetailScreen({
    required this.api,
    required this.session,
    required this.recipeId,
    super.key,
  });

  final CatchenApiApi api;
  final Session? session;
  final String recipeId;

  @override
  State<RecipeDetailScreen> createState() => _RecipeDetailScreenState();
}

class _RecipeDetailScreenState extends State<RecipeDetailScreen> {
  CatalogDetail? _detail;
  List<Map<String, dynamic>>? _comments;
  bool _isFavorite = false;
  bool _busyFavorite = false;
  final _commentController = TextEditingController();
  String? _error;

  @override
  void initState() {
    super.initState();
    _reload();
  }

  Map<String, dynamic>? _parseContent(CatalogDetail detail) {
    final json = detail.contentJson;
    if (json is! String) {
      return null;
    }
    try {
      return jsonDecode(json) as Map<String, dynamic>;
    } on FormatException {
      return null;
    }
  }

  Future<void> _reload() async {
    try {
      final detail = await widget.api.apiCatalogRecipesIdGet(widget.recipeId);
      final comments = await widget.api.apiCatalogRecipesIdCommentsGet(
        widget.recipeId,
      );

      var favorite = false;
      if (widget.session is Session) {
        final favorites = await widget.api.apiCatalogFavoritesGet();
        favorite =
            favorites?.items?.any((item) => item.recipeId == widget.recipeId) ==
            true;
      }

      if (!mounted) {
        return;
      }

      final commentRows = <Map<String, dynamic>>[
        for (final comment in comments?.comments ?? const <RecipeComment>[])
          if (comment.text is String) {'text': comment.text!},
      ];

      setState(() {
        _detail = detail;
        _comments = commentRows;
        _isFavorite = favorite;
        _error = null;
      });
    } catch (error) {
      if (!mounted) {
        return;
      }
      setState(() => _error = 'Could not load this recipe.');
    }
  }

  Future<void> _toggleFavorite() async {
    if (widget.session is! Session || _busyFavorite) {
      return;
    }
    setState(() => _busyFavorite = true);
    try {
      if (_isFavorite) {
        await widget.api.apiCatalogRecipesIdFavoriteDelete(widget.recipeId);
      } else {
        await widget.api.apiCatalogRecipesIdFavoritePost(widget.recipeId);
      }
      if (!mounted) {
        return;
      }
      setState(() {
        _isFavorite = !_isFavorite;
        _busyFavorite = false;
      });
    } catch (error) {
      if (!mounted) {
        return;
      }
      setState(() => _busyFavorite = false);
    }
  }

  Future<void> _submitComment() async {
    final text = _commentController.text.trim();
    if (text.isEmpty) {
      return;
    }
    try {
      final response = await widget.api.apiCatalogRecipesIdCommentsPost(
        widget.recipeId,
        CommentRequest(text: text),
      );
      if (!mounted) {
        return;
      }
      if (response?.id is String) {
        _commentController.clear();
        await _reload();
      } else {
        setState(() => _error = 'Comment could not be posted.');
      }
    } catch (error) {
      if (!mounted) {
        return;
      }
      setState(() => _error = 'Comment could not be posted.');
    }
  }

  @override
  void dispose() {
    _commentController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final detail = _detail;
    return Scaffold(
      appBar: AppBar(
        title: Text(detail?.summary?.title ?? 'Recipe'),
        actions: [
          if (widget.session is Session)
            IconButton(
              tooltip: _isFavorite ? 'Remove favorite' : 'Add favorite',
              icon: Icon(_isFavorite ? Icons.favorite : Icons.favorite_border),
              onPressed: _toggleFavorite,
            ),
        ],
      ),
      body: detail is CatalogDetail
          ? _buildDetail(context, detail)
          : _error is String
          ? Center(child: Text(_error!))
          : const Center(child: CircularProgressIndicator()),
    );
  }

  Widget _buildDetail(BuildContext context, CatalogDetail detail) {
    final summary = detail.summary;
    final content = _parseContent(detail);
    final locked = content == null;

    final ingredients = (content?['ingredients'] as List<dynamic>? ?? [])
        .cast<Map<String, dynamic>>();
    final instructions = (content?['instructions'] as List<dynamic>? ?? [])
        .cast<String>();
    final equipment = (content?['equipment'] as List<dynamic>? ?? [])
        .cast<String>();

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text(
          summary?.previewText ?? '',
          style: Theme.of(context).textTheme.bodyLarge,
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8,
          children: [
            Chip(label: Text(summary?.cuisine ?? '')),
            Chip(label: Text(summary?.difficulty ?? '')),
            if ((summary?.isFree ?? false) == false)
              const Chip(
                label: Text('Premium'),
                backgroundColor: Colors.amberAccent,
              ),
          ],
        ),
        const SizedBox(height: 16),
        if (locked) ...[
          const Card(
            child: Padding(
              padding: EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Full recipe locked',
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 8),
                  Text(
                    'Unlock with a monthly membership or a one-time purchase.',
                  ),
                ],
              ),
            ),
          ),
          for (final option in detail.purchaseOptions ?? const <String>[])
            ListTile(
              leading: const Icon(Icons.lock_open),
              title: Text(option.replaceAll('_', ' ')),
            ),
        ] else ...[
          const Text(
            'Ingredients',
            style: TextStyle(fontWeight: FontWeight.bold),
          ),
          for (final ingredient in ingredients)
            ListTile(
              dense: true,
              title: Text(ingredient['name']?.toString() ?? ''),
              subtitle: ingredient['substitution'] is Map<String, dynamic>
                  ? Text(
                      'Substitute: '
                      '${(ingredient['substitution'] as Map<String, dynamic>)['item']}',
                    )
                  : null,
              trailing: Text(_quantityLabel(ingredient['quantity'])),
            ),
          const SizedBox(height: 12),
          const Text(
            'Instructions',
            style: TextStyle(fontWeight: FontWeight.bold),
          ),
          for (var i = 0; i < instructions.length; i++)
            ListTile(
              dense: true,
              leading: CircleAvatar(child: Text('${i + 1}')),
              title: Text(instructions[i]),
            ),
          const SizedBox(height: 12),
          const Text(
            'Equipment',
            style: TextStyle(fontWeight: FontWeight.bold),
          ),
          Wrap(
            spacing: 8,
            children: [for (final tool in equipment) Chip(label: Text(tool))],
          ),
          const SizedBox(height: 12),
          const Text(
            'Cultural context',
            style: TextStyle(fontWeight: FontWeight.bold),
          ),
          Text(content['culturalContext']?.toString() ?? ''),
        ],
        const Divider(height: 32),
        const Text('Comments', style: TextStyle(fontWeight: FontWeight.bold)),
        if (widget.session is Session)
          TextField(
            controller: _commentController,
            decoration: InputDecoration(
              labelText: 'Write a comment',
              suffixIcon: IconButton(
                icon: const Icon(Icons.send),
                onPressed: _submitComment,
              ),
            ),
          )
        else
          const Padding(
            padding: EdgeInsets.symmetric(vertical: 8),
            child: Text('Sign in to join the conversation.'),
          ),
        for (final comment in _comments ?? const <Map<String, dynamic>>[])
          ListTile(
            dense: true,
            leading: const Icon(Icons.comment),
            title: Text(comment['text'] ?? ''),
          ),
      ],
    );
  }

  String _quantityLabel(dynamic quantity) {
    if (quantity is! Map<String, dynamic>) {
      return '';
    }
    return '${quantity['value'] ?? ''} ${quantity['unit'] ?? ''}'.trim();
  }
}
