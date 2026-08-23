//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class ProvenanceEvidence {
  /// Returns a new [ProvenanceEvidence] instance.
  ProvenanceEvidence({
    this.originalTextAttested,
    this.originalPhotographyAttested,
    required this.sourceNote,
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  bool? originalTextAttested;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  bool? originalPhotographyAttested;

  String? sourceNote;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ProvenanceEvidence &&
          other.originalTextAttested == originalTextAttested &&
          other.originalPhotographyAttested == originalPhotographyAttested &&
          other.sourceNote == sourceNote;

  @override
  int get hashCode =>
      // ignore: unnecessary_parenthesis
      (originalTextAttested == null ? 0 : originalTextAttested!.hashCode) +
      (originalPhotographyAttested == null
          ? 0
          : originalPhotographyAttested!.hashCode) +
      (sourceNote == null ? 0 : sourceNote!.hashCode);

  @override
  String toString() =>
      'ProvenanceEvidence[originalTextAttested=$originalTextAttested, originalPhotographyAttested=$originalPhotographyAttested, sourceNote=$sourceNote]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.originalTextAttested != null) {
      json[r'originalTextAttested'] = this.originalTextAttested;
    } else {
      json[r'originalTextAttested'] = null;
    }
    if (this.originalPhotographyAttested != null) {
      json[r'originalPhotographyAttested'] = this.originalPhotographyAttested;
    } else {
      json[r'originalPhotographyAttested'] = null;
    }
    if (this.sourceNote != null) {
      json[r'sourceNote'] = this.sourceNote;
    } else {
      json[r'sourceNote'] = null;
    }
    return json;
  }

  /// Returns a new [ProvenanceEvidence] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static ProvenanceEvidence? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key),
              'Required key "ProvenanceEvidence[$key]" is missing from JSON.');
          assert(json[key] != null,
              'Required key "ProvenanceEvidence[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return ProvenanceEvidence(
        originalTextAttested:
            mapValueOfType<bool>(json, r'originalTextAttested'),
        originalPhotographyAttested:
            mapValueOfType<bool>(json, r'originalPhotographyAttested'),
        sourceNote: mapValueOfType<String>(json, r'sourceNote'),
      );
    }
    return null;
  }

  static List<ProvenanceEvidence> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <ProvenanceEvidence>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = ProvenanceEvidence.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, ProvenanceEvidence> mapFromJson(dynamic json) {
    final map = <String, ProvenanceEvidence>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = ProvenanceEvidence.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of ProvenanceEvidence-objects as value to a dart map
  static Map<String, List<ProvenanceEvidence>> mapListFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final map = <String, List<ProvenanceEvidence>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = ProvenanceEvidence.listFromJson(
          entry.value,
          growable: growable,
        );
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'sourceNote',
  };
}
