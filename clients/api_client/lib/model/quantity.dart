//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class Quantity {
  /// Returns a new [Quantity] instance.
  Quantity({
    required this.value,
    required this.unit,
  });

  double value;

  String? unit;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is Quantity && other.value == value && other.unit == unit;

  @override
  int get hashCode =>
      // ignore: unnecessary_parenthesis
      (value.hashCode) + (unit == null ? 0 : unit!.hashCode);

  @override
  String toString() => 'Quantity[value=$value, unit=$unit]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    json[r'value'] = this.value;
    if (this.unit != null) {
      json[r'unit'] = this.unit;
    } else {
      json[r'unit'] = null;
    }
    return json;
  }

  /// Returns a new [Quantity] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static Quantity? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key),
              'Required key "Quantity[$key]" is missing from JSON.');
          assert(json[key] != null,
              'Required key "Quantity[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return Quantity(
        value: mapValueOfType<double>(json, r'value')!,
        unit: mapValueOfType<String>(json, r'unit'),
      );
    }
    return null;
  }

  static List<Quantity> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <Quantity>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = Quantity.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, Quantity> mapFromJson(dynamic json) {
    final map = <String, Quantity>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = Quantity.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of Quantity-objects as value to a dart map
  static Map<String, List<Quantity>> mapListFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final map = <String, List<Quantity>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = Quantity.listFromJson(
          entry.value,
          growable: growable,
        );
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'value',
    'unit',
  };
}
