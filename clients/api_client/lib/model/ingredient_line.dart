//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class IngredientLine {
  /// Returns a new [IngredientLine] instance.
  IngredientLine({
    required this.name,
    required this.quantity,
    this.substitution,
  });

  String? name;

  Quantity quantity;

  ///
  /// Please note: This property should have been non-nullable! Since the specification file
  /// does not include a default value (using the "default:" property), however, the generated
  /// source code must fall back to having a nullable type.
  /// Consider adding a "default:" property in the specification file to hide this note.
  ///
  Substitution? substitution;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is IngredientLine &&
          other.name == name &&
          other.quantity == quantity &&
          other.substitution == substitution;

  @override
  int get hashCode =>
      // ignore: unnecessary_parenthesis
      (name == null ? 0 : name!.hashCode) +
      (quantity.hashCode) +
      (substitution == null ? 0 : substitution!.hashCode);

  @override
  String toString() =>
      'IngredientLine[name=$name, quantity=$quantity, substitution=$substitution]';

  Map<String, dynamic> toJson() {
    final json = <String, dynamic>{};
    if (this.name != null) {
      json[r'name'] = this.name;
    } else {
      json[r'name'] = null;
    }
    json[r'quantity'] = this.quantity;
    if (this.substitution != null) {
      json[r'substitution'] = this.substitution;
    } else {
      json[r'substitution'] = null;
    }
    return json;
  }

  /// Returns a new [IngredientLine] instance and imports its values from
  /// [value] if it's a [Map], null otherwise.
  // ignore: prefer_constructors_over_static_methods
  static IngredientLine? fromJson(dynamic value) {
    if (value is Map) {
      final json = value.cast<String, dynamic>();

      // Ensure that the map contains the required keys.
      // Note 1: the values aren't checked for validity beyond being non-null.
      // Note 2: this code is stripped in release mode!
      assert(() {
        requiredKeys.forEach((key) {
          assert(json.containsKey(key),
              'Required key "IngredientLine[$key]" is missing from JSON.');
          assert(json[key] != null,
              'Required key "IngredientLine[$key]" has a null value in JSON.');
        });
        return true;
      }());

      return IngredientLine(
        name: mapValueOfType<String>(json, r'name'),
        quantity: Quantity.fromJson(json[r'quantity'])!,
        substitution: Substitution.fromJson(json[r'substitution']),
      );
    }
    return null;
  }

  static List<IngredientLine> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <IngredientLine>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = IngredientLine.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }

  static Map<String, IngredientLine> mapFromJson(dynamic json) {
    final map = <String, IngredientLine>{};
    if (json is Map && json.isNotEmpty) {
      json = json.cast<String, dynamic>(); // ignore: parameter_assignments
      for (final entry in json.entries) {
        final value = IngredientLine.fromJson(entry.value);
        if (value != null) {
          map[entry.key] = value;
        }
      }
    }
    return map;
  }

  // maps a json object with a list of IngredientLine-objects as value to a dart map
  static Map<String, List<IngredientLine>> mapListFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final map = <String, List<IngredientLine>>{};
    if (json is Map && json.isNotEmpty) {
      // ignore: parameter_assignments
      json = json.cast<String, dynamic>();
      for (final entry in json.entries) {
        map[entry.key] = IngredientLine.listFromJson(
          entry.value,
          growable: growable,
        );
      }
    }
    return map;
  }

  /// The list of required keys that must be present in a JSON.
  static const requiredKeys = <String>{
    'name',
    'quantity',
  };
}
