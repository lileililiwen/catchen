//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class ApprovedChannel {
  /// Returns a new [ApprovedChannel] instance.
  ApprovedChannel({
    this.id,
    required this.channel,
    required this.kind,
    required this.approvedByUserId,
    this.approvedAtUtc,
  });

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  String? id;

  String? channel;

  String? kind;

  String approvedByUserId;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  DateTime? approvedAtUtc;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ApprovedChannel &&
          other.id == id &&
          other.channel == channel &&
          other.kind == kind &&
          other.approvedByUserId == approvedByUserId &&
          other.approvedAtUtc == approvedAtUtc;

  @override
  int get hashCode =>
      // ignore: unnecessary_parenthesis
      (id == null ? 0 : id!.hashCode) +
      (channel == null ? 0 : channel!.hashCode) +
      (kind == null ? 0 : kind!.hashCode) +
      (approvedByUserId.hashCode) +
      (approvedAtUtc == null ? 0 : approvedAtUtc!.hashCode);

  @override
  String toString() =>
      'ApprovedChannel[id=$id, channel=$channel, kind=$kind, approvedByUserId=$approvedByUserId, approvedAtUtc=$approvedAtUtc]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.id != null) {
      json[r'id'] = this.id;
    } else {
      json[r'id'] = null;
    }
    if (this.channel != null) {
      json[r'channel'] = this.channel;
    } else {
      json[r'channel'] = null;
    }
    if (this.kind != null) {
      json[r'kind'] = this.kind;
    } else {
      json[r'kind'] = null;
    }
    json[r'approvedByUserId'] = this.approvedByUserId;
    if (this.approvedAtUtc != null) {
      json[r'approvedAtUtc'] = this.approvedAtUtc!.toUtc().toIso8601String();
    } else {
      json[r'approvedAtUtc'] = null;
    }
    return json;
  }

  /// Returns a new [ApprovedChannel] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static ApprovedChannel? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key),
              'Required key "ApprovedChannel[$key]" is missing from JSON.');
          assert(json[key] != null,
              'Required key "ApprovedChannel[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return ApprovedChannel(
        id: mapValueOfType<String>(json, r'id'),
        channel: mapValueOfType<String>(json, r'channel'),
        kind: mapValueOfType<String>(json, r'kind'),
        approvedByUserId: mapValueOfType<String>(json, r'approvedByUserId')!,
        approvedAtUtc: mapDateTime(json, r'approvedAtUtc', r''),
      );
    }
    return null;
  }

  static List<ApprovedChannel> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <ApprovedChannel>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = ApprovedChannel.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, ApprovedChannel> mapFromJson(dynamic json) {
    final map = <String, ApprovedChannel>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = ApprovedChannel.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of ApprovedChannel-objects as value to a dart map
  static Map<String, List<ApprovedChannel>> mapListFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final map = <String, List<ApprovedChannel>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = ApprovedChannel.listFromJson(
          entry.value,
          growable: growable,
        );
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'channel',
    'kind',
    'approvedByUserId',
  };
}
