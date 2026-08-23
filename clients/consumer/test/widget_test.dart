import 'package:catchen_api_client/api.dart';
import 'package:catchen_consumer/main.dart' as app;
import 'package:catchen_consumer/recipe_detail_screen.dart';
import 'package:catchen_consumer/session.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';

const _userToken =
    'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0IiwiZW1haWwiOiJ1c2VyQGV4YW1wbGUuY29t'
    'Iiwicm9sZSI6IlJlZ3VsYXJVc2VyIn0.ignored';

class _MockApi extends Mock implements CatchenApiApi {}

CatalogSummary _summary(String title, {bool free = true}) => CatalogSummary()
  ..recipeId = 'recipe-$title'
  ..title = title
  ..cuisine = 'sichuan'
  ..difficulty = 'medium'
  ..previewText = 'Preview of $title'
  ..isFree = free;

void main() {
  setUp(() {
    Session.current = null;
    registerFallbackValue(CommentRequest(text: 'x'));
  });

  testWidgets('catalog lists published recipes with premium badge', (
    tester,
  ) async {
    final api = _MockApi();
    when(
      () => api.apiCatalogRecipesGet(
        category: any(named: 'category'),
        difficulty: any(named: 'difficulty'),
        ingredient: any(named: 'ingredient'),
        q: any(named: 'q'),
      ),
    ).thenAnswer(
      (_) async => CatalogListResponse(
        items: [
          _summary('Free Stir Fry'),
          _summary('Premium Banquet', free: false),
        ],
      ),
    );

    Session.current = Session.fromToken(_userToken);
    await tester.pumpWidget(app.MainApp(loginApi: api, catalogApi: api));
    await tester.pump();
    debugPrint(
      'ERR=${find.text("Could not load recipes.").evaluate().length} '
      'SPIN=${find.byType(CircularProgressIndicator).evaluate().length} '
      'ITEMS=${find.text("Free Stir Fry").evaluate().length}',
    );
    await tester.pump();
    debugPrint(
      '2nd ERR=${find.text("Could not load recipes.").evaluate().length} '
      'SPIN=${find.byType(CircularProgressIndicator).evaluate().length} '
      'ITEMS=${find.text("Free Stir Fry").evaluate().length}',
    );

    expect(find.text('Free Stir Fry'), findsOneWidget);
    expect(find.text('Premium Banquet'), findsOneWidget);
    expect(find.textContaining('· Premium'), findsOneWidget);
  });

  testWidgets('locked premium detail shows purchase options, not content', (
    tester,
  ) async {
    final api = _MockApi();
    when(
      () => api.apiCatalogRecipesGet(
        category: any(named: 'category'),
        difficulty: any(named: 'difficulty'),
        ingredient: any(named: 'ingredient'),
        q: any(named: 'q'),
      ),
    ).thenAnswer(
      (_) async => CatalogListResponse(
        items: [_summary('Premium Banquet', free: false)],
      ),
    );
    when(() => api.apiCatalogRecipesIdGet('recipe-Premium Banquet')).thenAnswer(
      (_) async => CatalogDetail()
        ..summary = _summary('Premium Banquet', free: false)
        ..purchaseOptions = ['membership', 'single_recipe_purchase'],
    );
    when(() => api.apiCatalogRecipesIdCommentsGet('recipe-Premium Banquet'))
        .thenAnswer((_) async => CommentListResponse(comments: const []));
    when(() => api.apiCatalogFavoritesGet())
        .thenAnswer((_) async => CatalogListResponse(items: const []));

    Session.current = Session.fromToken(_userToken);
    await tester.pumpWidget(app.MainApp(loginApi: api, catalogApi: api));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Premium Banquet'));
    await tester.pumpAndSettle();
    debugPrint(
      'DETAIL=${find.byType(RecipeDetailScreen).evaluate().length} '
      'ERR=${find.text("Could not load this recipe.").evaluate().length}',
    );

    expect(find.text('Full recipe locked'), findsOneWidget);
    expect(find.text('membership'), findsOneWidget);
    expect(find.textContaining('SECRET STEP'), findsNothing);
  });

  testWidgets('free detail renders ingredients and instructions', (
    tester,
  ) async {
    final api = _MockApi();
    when(
      () => api.apiCatalogRecipesGet(
        category: any(named: 'category'),
        difficulty: any(named: 'difficulty'),
        ingredient: any(named: 'ingredient'),
        q: any(named: 'q'),
      ),
    ).thenAnswer(
      (_) async => CatalogListResponse(items: [_summary('Free Stir Fry')]),
    );
    when(() => api.apiCatalogRecipesIdGet('recipe-Free Stir Fry')).thenAnswer(
      (_) async => CatalogDetail()
        ..summary = _summary('Free Stir Fry')
        ..contentJson =
            '{"ingredients":[{"name":"tofu","quantity":{"value":400,"unit":"g"}}],'
            '"instructions":["Cut tofu."],"equipment":["wok"],'
            '"culturalContext":"Sichuan classic"}',
    );
    when(() => api.apiCatalogRecipesIdCommentsGet('recipe-Free Stir Fry'))
        .thenAnswer((_) async => CommentListResponse(comments: const []));
    when(() => api.apiCatalogFavoritesGet())
        .thenAnswer((_) async => CatalogListResponse(items: const []));

    Session.current = Session.fromToken(_userToken);
    await tester.pumpWidget(app.MainApp(loginApi: api, catalogApi: api));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Free Stir Fry'));
    await tester.pumpAndSettle();

    expect(find.text('Ingredients'), findsOneWidget);
    expect(find.text('400 g'), findsOneWidget);
    expect(find.text('Cut tofu.'), findsOneWidget);
    expect(find.text('Sichuan classic'), findsOneWidget);
  });

  testWidgets('favorites tab lists saved recipes', (tester) async {
    final api = _MockApi();
    when(
      () => api.apiCatalogRecipesGet(
        category: any(named: 'category'),
        difficulty: any(named: 'difficulty'),
        ingredient: any(named: 'ingredient'),
        q: any(named: 'q'),
      ),
    ).thenAnswer((_) async => CatalogListResponse(items: const []));
    when(() => api.apiCatalogFavoritesGet()).thenAnswer(
      (_) async => CatalogListResponse(items: [_summary('Favored Dish')]),
    );

    Session.current = Session.fromToken(_userToken);
    await tester.pumpWidget(app.MainApp(loginApi: api, catalogApi: api));
    await tester.pumpAndSettle();
    await tester.tap(find.byIcon(Icons.favorite));
    await tester.pumpAndSettle();

    expect(find.text('Favored Dish'), findsOneWidget);
  });
}
