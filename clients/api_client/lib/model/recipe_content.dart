//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class RecipeContent {
  /// Returns a new [RecipeContent] instance.
  RecipeContent({
    required this.title,
    required this.cuisine,
    required this.difficulty,
    required this.previewText,
    this.ingredients = const [],
    this.instructions = const [],
    this.equipment = const [],
    required this.culturalContext,
  });

  String? title;

  String? cuisine;

  String? difficulty;

  String? previewText;

  List<IngredientLine>? ingredients;

  List<String>? instructions;

  List<String>? equipment;

  String? culturalContext;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is RecipeContent &&
          other.title == title &&
          other.cuisine == cuisine &&
          other.difficulty == difficulty &&
          other.previewText == previewText &&
          _deepEquality.equals(other.ingredients, ingredients) &&
          _deepEquality.equals(other.instructions, instructions) &&
          _deepEquality.equals(other.equipment, equipment) &&
          other.culturalContext == culturalContext;

  @override
  int get hashCode =>
      // ignore: unnecessary_parenthesis
      (title == null ? 0 : title!.hashCode) +
      (cuisine == null ? 0 : cuisine!.hashCode) +
      (difficulty == null ? 0 : difficulty!.hashCode) +
      (previewText == null ? 0 : previewText!.hashCode) +
      (ingredients == null ? 0 : ingredients!.hashCode) +
      (instructions == null ? 0 : instructions!.hashCode) +
      (equipment == null ? 0 : equipment!.hashCode) +
      (culturalContext == null ? 0 : culturalContext!.hashCode);

  @override
  String toString() =>
      'RecipeContent[title=$title, cuisine=$cuisine, difficulty=$difficulty, previewText=$previewText, ingredients=$ingredients, instructions=$instructions, equipment=$equipment, culturalContext=$culturalContext]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.title != null) {
      json[r'title'] = this.title;
    } else {
      json[r'title'] = null;
    }
    if (this.cuisine != null) {
      json[r'cuisine'] = this.cuisine;
    } else {
      json[r'cuisine'] = null;
    }
    if (this.difficulty != null) {
      json[r'difficulty'] = this.difficulty;
    } else {
      json[r'difficulty'] = null;
    }
    if (this.previewText != null) {
      json[r'previewText'] = this.previewText;
    } else {
      json[r'previewText'] = null;
    }
    if (this.ingredients != null) {
      json[r'ingredients'] = this.ingredients;
    } else {
      json[r'ingredients'] = null;
    }
    if (this.instructions != null) {
      json[r'instructions'] = this.instructions;
    } else {
      json[r'instructions'] = null;
    }
    if (this.equipment != null) {
      json[r'equipment'] = this.equipment;
    } else {
      json[r'equipment'] = null;
    }
    if (this.culturalContext != null) {
      json[r'culturalContext'] = this.culturalContext;
    } else {
      json[r'culturalContext'] = null;
    }
    return json;
  }

  /// Returns a new [RecipeContent] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static RecipeContent? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key),
              'Required key "RecipeContent[$key]" is missing from JSON.');
          assert(json[key] != null,
              'Required key "RecipeContent[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return RecipeContent(
        title: mapValueOfType<String>(json, r'title'),
        cuisine: mapValueOfType<String>(json, r'cuisine'),
        difficulty: mapValueOfType<String>(json, r'difficulty'),
        previewText: mapValueOfType<String>(json, r'previewText'),
        ingredients: IngredientLine.listFromJson(json[r'ingredients']),
        instructions: json[r'instructions'] is Iterable
            ? (json[r'instructions'] as Iterable)
                .cast<String>()
                .toList(growable: false)
            : const [],
        equipment: json[r'equipment'] is Iterable
            ? (json[r'equipment'] as Iterable)
                .cast<String>()
                .toList(growable: false)
            : const [],
        culturalContext: mapValueOfType<String>(json, r'culturalContext'),
      );
    }
    return null;
  }

  static List<RecipeContent> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <RecipeContent>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = RecipeContent.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, RecipeContent> mapFromJson(dynamic json) {
    final map = <String, RecipeContent>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = RecipeContent.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of RecipeContent-objects as value to a dart map
  static Map<String, List<RecipeContent>> mapListFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final map = <String, List<RecipeContent>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = RecipeContent.listFromJson(
          entry.value,
          growable: growable,
        );
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'title',
    'cuisine',
    'difficulty',
    'previewText',
    'ingredients',
    'instructions',
    'equipment',
    'culturalContext',
  };
}
