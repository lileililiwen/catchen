//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class ApproveChannelRequest {
  /// Returns a new [ApproveChannelRequest] instance.
  ApproveChannelRequest({
    this.channel,
    this.kind,
  });

  String? channel;

  String? kind;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ApproveChannelRequest &&
          other.channel == channel &&
          other.kind == kind;

  @override
  int get hashCode =>
      // ignore: unnecessary_parenthesis
      (channel == null ? 0 : channel!.hashCode) +
      (kind == null ? 0 : kind!.hashCode);

  @override
  String toString() => 'ApproveChannelRequest[channel=$channel, kind=$kind]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
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
    return json;
  }

  /// Returns a new [ApproveChannelRequest] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static ApproveChannelRequest? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key),
              'Required key "ApproveChannelRequest[$key]" is missing from JSON.');
          assert(json[key] != null,
              'Required key "ApproveChannelRequest[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return ApproveChannelRequest(
        channel: mapValueOfType<String>(json, r'channel'),
        kind: mapValueOfType<String>(json, r'kind'),
      );
    }
    return null;
  }

  static List<ApproveChannelRequest> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <ApproveChannelRequest>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = ApproveChannelRequest.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, ApproveChannelRequest> mapFromJson(dynamic json) {
    final map = <String, ApproveChannelRequest>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = ApproveChannelRequest.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of ApproveChannelRequest-objects as value to a dart map
  static Map<String, List<ApproveChannelRequest>> mapListFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final map = <String, List<ApproveChannelRequest>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = ApproveChannelRequest.listFromJson(
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
