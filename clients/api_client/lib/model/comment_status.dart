//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class CommentStatus {
  /// Instantiate a new enum with the provided [value].
  const CommentStatus._(this.value);

  /// The underlying value of this enum member.
  final int value;

  @override
  String toString() => value.toString();

  int toJson() => value;

  static const number0 = CommentStatus._(0);
  static const number1 = CommentStatus._(1);

  /// List of all possible values in this [enum][CommentStatus].
  static const values = <CommentStatus>[
    number0,
    number1,
  ];

  static CommentStatus? fromJson(dynamic value) =>
      CommentStatusTypeTransformer().decode(value);

  static List<CommentStatus> listFromJson(
    dynamic json, {
    bool growable = false,
  }) {
    final result = <CommentStatus>[];
    if (json is List && json.isNotEmpty) {
      for (final row in json) {
        final value = CommentStatus.fromJson(row);
        if (value != null) {
          result.add(value);
        }
      }
    }
    return result.toList(growable: growable);
  }
}

/// Transformation class that can [encode] an instance of [CommentStatus] to int,
/// and [decode] dynamic data back to [CommentStatus].
class CommentStatusTypeTransformer {
  factory CommentStatusTypeTransformer() =>
      _instance ??= const CommentStatusTypeTransformer._();

  const CommentStatusTypeTransformer._();

  int encode(CommentStatus data) => data.value;

  /// Decodes a [dynamic value][data] to a CommentStatus.
  ///
  /// If [allowNull] is true and the [dynamic value][data] cannot be decoded successfully,
  /// then null is returned. However, if [allowNull] is false and the [dynamic value][data]
  /// cannot be decoded successfully, then an [UnimplementedError] is thrown.
  ///
  /// The [allowNull] is very handy when an API changes and a new enum value is added or removed,
  /// and users are still using an old app with the old code.
  CommentStatus? decode(dynamic data, {bool allowNull = true}) {
    if (data != null) {
      switch (data) {
        case 0:
          return CommentStatus.number0;
        case 1:
          return CommentStatus.number1;
        default:
          if (!allowNull) {
            throw ArgumentError('Unknown enum value to decode: $data');
          }
      }
    }
    return null;
  }

  /// Singleton [CommentStatusTypeTransformer] instance.
  static CommentStatusTypeTransformer? _instance;
}
