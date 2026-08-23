//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class CreateDraftRequest {
  /// Returns a new [CreateDraftRequest] instance.
  CreateDraftRequest({
    this.content,
    this.provenance,
    this.isFree,
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  RecipeContent? content;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  ProvenanceEvidence? provenance;

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
      other is CreateDraftRequest &&
          other.content == content &&
          other.provenance == provenance &&
          other.isFree == isFree;

  @override
  int get hashCode =>
      // ignore: unnecessary_parenthesis
      (content == null ? 0 : content!.hashCode) +
      (provenance == null ? 0 : provenance!.hashCode) +
      (isFree == null ? 0 : isFree!.hashCode);

  @override
  String toString() =>
      'CreateDraftRequest[content=$content, provenance=$provenance, isFree=$isFree]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.content != null) {
      json[r'content'] = this.content;
    } else {
      json[r'content'] = null;
    }
    if (this.provenance != null) {
      json[r'provenance'] = this.provenance;
    } else {
      json[r'provenance'] = null;
    }
    if (this.isFree != null) {
      json[r'isFree'] = this.isFree;
    } else {
      json[r'isFree'] = null;
    }
    return json;
  }

  /// Returns a new [CreateDraftRequest] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static CreateDraftRequest? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key),
              'Required key "CreateDraftRequest[$key]" is missing from JSON.');
          assert(json[key] != null,
              'Required key "CreateDraftRequest[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return CreateDraftRequest(
        content: RecipeContent.fromJson(json[r'content']),
        provenance: ProvenanceEvidence.fromJson(json[r'provenance']),
        isFree: mapValueOfType<bool>(json, r'isFree'),
      );
    }
    return null;
  }

  static List<CreateDraftRequest> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <CreateDraftRequest>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = CreateDraftRequest.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, CreateDraftRequest> mapFromJson(dynamic json) {
    final map = <String, CreateDraftRequest>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = CreateDraftRequest.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of CreateDraftRequest-objects as value to a dart map
  static Map<String, List<CreateDraftRequest>> mapListFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final map = <String, List<CreateDraftRequest>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = CreateDraftRequest.listFromJson(
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
