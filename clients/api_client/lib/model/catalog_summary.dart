//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class CatalogSummary {
  /// Returns a new [CatalogSummary] instance.
  CatalogSummary({
    this.recipeId,
    this.version,
    this.title,
    this.cuisine,
    this.difficulty,
    this.previewText,
    this.isFree,
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? recipeId;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  int? version;

  String? title;

  String? cuisine;

  String? difficulty;

  String? previewText;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  bool? isFree;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is CatalogSummary &&
          other.recipeId == recipeId &&
          other.version == version &&
          other.title == title &&
          other.cuisine == cuisine &&
          other.difficulty == difficulty &&
          other.previewText == previewText &&
          other.isFree == isFree;

  @override
  int get hashCode =>
      // ignore: unnecessary_parenthesis
      (recipeId == null ? 0 : recipeId!.hashCode) +
      (version == null ? 0 : version!.hashCode) +
      (title == null ? 0 : title!.hashCode) +
      (cuisine == null ? 0 : cuisine!.hashCode) +
      (difficulty == null ? 0 : difficulty!.hashCode) +
      (previewText == null ? 0 : previewText!.hashCode) +
      (isFree == null ? 0 : isFree!.hashCode);

  @override
  String toString() =>
      'CatalogSummary[recipeId=$recipeId, version=$version, title=$title, cuisine=$cuisine, difficulty=$difficulty, previewText=$previewText, isFree=$isFree]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.recipeId != null) {
      json[r'recipeId'] = this.recipeId;
    } else {
      json[r'recipeId'] = null;
    }
    if (this.version != null) {
      json[r'version'] = this.version;
    } else {
      json[r'version'] = null;
    }
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
    if (this.isFree != null) {
      json[r'isFree'] = this.isFree;
    } else {
      json[r'isFree'] = null;
    }
    return json;
  }

  /// Returns a new [CatalogSummary] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static CatalogSummary? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key),
              'Required key "CatalogSummary[$key]" is missing from JSON.');
          assert(json[key] != null,
              'Required key "CatalogSummary[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return CatalogSummary(
        recipeId: mapValueOfType<String>(json, r'recipeId'),
        version: mapValueOfType<int>(json, r'version'),
        title: mapValueOfType<String>(json, r'title'),
        cuisine: mapValueOfType<String>(json, r'cuisine'),
        difficulty: mapValueOfType<String>(json, r'difficulty'),
        previewText: mapValueOfType<String>(json, r'previewText'),
        isFree: mapValueOfType<bool>(json, r'isFree'),
      );
    }
    return null;
  }

  static List<CatalogSummary> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <CatalogSummary>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = CatalogSummary.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, CatalogSummary> mapFromJson(dynamic json) {
    final map = <String, CatalogSummary>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = CatalogSummary.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of CatalogSummary-objects as value to a dart map
  static Map<String, List<CatalogSummary>> mapListFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final map = <String, List<CatalogSummary>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = CatalogSummary.listFromJson(
          entry.value,
          growable: growable,
        );
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{};
}
