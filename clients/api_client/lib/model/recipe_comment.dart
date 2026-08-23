//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class RecipeComment {
  /// Returns a new [RecipeComment] instance.
  RecipeComment({
    this.id,
    required this.recipeId,
    required this.userId,
    required this.text,
    this.status,
    this.moderationReason,
    this.createdAtUtc,
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? id;

  String recipeId;

  String userId;

  String? text;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  CommentStatus? status;

  String? moderationReason;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  DateTime? createdAtUtc;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is RecipeComment &&
          other.id == id &&
          other.recipeId == recipeId &&
          other.userId == userId &&
          other.text == text &&
          other.status == status &&
          other.moderationReason == moderationReason &&
          other.createdAtUtc == createdAtUtc;

  @override
  int get hashCode =>
      // ignore: unnecessary_parenthesis
      (id == null ? 0 : id!.hashCode) +
      (recipeId.hashCode) +
      (userId.hashCode) +
      (text == null ? 0 : text!.hashCode) +
      (status == null ? 0 : status!.hashCode) +
      (moderationReason == null ? 0 : moderationReason!.hashCode) +
      (createdAtUtc == null ? 0 : createdAtUtc!.hashCode);

  @override
  String toString() =>
      'RecipeComment[id=$id, recipeId=$recipeId, userId=$userId, text=$text, status=$status, moderationReason=$moderationReason, createdAtUtc=$createdAtUtc]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.id != null) {
      json[r'id'] = this.id;
    } else {
      json[r'id'] = null;
    }
    json[r'recipeId'] = this.recipeId;
    json[r'userId'] = this.userId;
    if (this.text != null) {
      json[r'text'] = this.text;
    } else {
      json[r'text'] = null;
    }
    if (this.status != null) {
      json[r'status'] = this.status;
    } else {
      json[r'status'] = null;
    }
    if (this.moderationReason != null) {
      json[r'moderationReason'] = this.moderationReason;
    } else {
      json[r'moderationReason'] = null;
    }
    if (this.createdAtUtc != null) {
      json[r'createdAtUtc'] = this.createdAtUtc!.toUtc().toIso8601String();
    } else {
      json[r'createdAtUtc'] = null;
    }
    return json;
  }

  /// Returns a new [RecipeComment] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static RecipeComment? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key),
              'Required key "RecipeComment[$key]" is missing from JSON.');
          assert(json[key] != null,
              'Required key "RecipeComment[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return RecipeComment(
        id: mapValueOfType<String>(json, r'id'),
        recipeId: mapValueOfType<String>(json, r'recipeId')!,
        userId: mapValueOfType<String>(json, r'userId')!,
        text: mapValueOfType<String>(json, r'text'),
        status: CommentStatus.fromJson(json[r'status']),
        moderationReason: mapValueOfType<String>(json, r'moderationReason'),
        createdAtUtc: mapDateTime(json, r'createdAtUtc', r''),
      );
    }
    return null;
  }

  static List<RecipeComment> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <RecipeComment>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = RecipeComment.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, RecipeComment> mapFromJson(dynamic json) {
    final map = <String, RecipeComment>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = RecipeComment.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of RecipeComment-objects as value to a dart map
  static Map<String, List<RecipeComment>> mapListFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final map = <String, List<RecipeComment>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = RecipeComment.listFromJson(
          entry.value,
          growable: growable,
        );
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'recipeId',
    'userId',
    'text',
  };
}
