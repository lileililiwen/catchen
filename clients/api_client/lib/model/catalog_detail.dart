//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class CatalogDetail {
  /// Returns a new [CatalogDetail] instance.
  CatalogDetail({
    this.summary,
    this.contentJson,
    this.purchaseOptions = const [],
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  CatalogSummary? summary;

  String? contentJson;

  List<String>? purchaseOptions;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is CatalogDetail &&
          other.summary == summary &&
          other.contentJson == contentJson &&
          _deepEquality.equals(other.purchaseOptions, purchaseOptions);

  @override
  int get hashCode =>
      // ignore: unnecessary_parenthesis
      (summary == null ? 0 : summary!.hashCode) +
      (contentJson == null ? 0 : contentJson!.hashCode) +
      (purchaseOptions == null ? 0 : purchaseOptions!.hashCode);

  @override
  String toString() =>
      'CatalogDetail[summary=$summary, contentJson=$contentJson, purchaseOptions=$purchaseOptions]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.summary != null) {
      json[r'summary'] = this.summary;
    } else {
      json[r'summary'] = null;
    }
    if (this.contentJson != null) {
      json[r'contentJson'] = this.contentJson;
    } else {
      json[r'contentJson'] = null;
    }
    if (this.purchaseOptions != null) {
      json[r'purchaseOptions'] = this.purchaseOptions;
    } else {
      json[r'purchaseOptions'] = null;
    }
    return json;
  }

  /// Returns a new [CatalogDetail] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static CatalogDetail? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key),
              'Required key "CatalogDetail[$key]" is missing from JSON.');
          assert(json[key] != null,
              'Required key "CatalogDetail[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return CatalogDetail(
        summary: CatalogSummary.fromJson(json[r'summary']),
        contentJson: mapValueOfType<String>(json, r'contentJson'),
        purchaseOptions: json[r'purchaseOptions'] is Iterable
            ? (json[r'purchaseOptions'] as Iterable)
                .cast<String>()
                .toList(growable: false)
            : const [],
      );
    }
    return null;
  }

  static List<CatalogDetail> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <CatalogDetail>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = CatalogDetail.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, CatalogDetail> mapFromJson(dynamic json) {
    final map = <String, CatalogDetail>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = CatalogDetail.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of CatalogDetail-objects as value to a dart map
  static Map<String, List<CatalogDetail>> mapListFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final map = <String, List<CatalogDetail>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = CatalogDetail.listFromJson(
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
