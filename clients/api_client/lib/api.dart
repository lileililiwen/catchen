//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

library openapi.api;

import 'dart:async';
import 'dart:convert';
import 'dart:io';

import 'package:collection/collection.dart';
import 'package:http/http.dart';
import 'package:intl/intl.dart';
import 'package:meta/meta.dart';

part 'api_client.dart';
part 'api_helper.dart';
part 'api_exception.dart';
part 'auth/authentication.dart';
part 'auth/api_key_auth.dart';
part 'auth/oauth.dart';
part 'auth/http_basic_auth.dart';
part 'auth/http_bearer_auth.dart';

part 'api/catchen_api_api.dart';

part 'model/approval_response.dart';
part 'model/approve_channel_request.dart';
part 'model/approved_channel.dart';
part 'model/approved_channels_response.dart';
part 'model/catalog_detail.dart';
part 'model/catalog_list_response.dart';
part 'model/catalog_summary.dart';
part 'model/comment_created_response.dart';
part 'model/comment_list_response.dart';
part 'model/comment_request.dart';
part 'model/comment_status.dart';
part 'model/create_draft_request.dart';
part 'model/ingredient_line.dart';
part 'model/login_endpoint_request.dart';
part 'model/login_response.dart';
part 'model/payment_methods_response.dart';
part 'model/provenance_evidence.dart';
part 'model/quantity.dart';
part 'model/reason_code_request.dart';
part 'model/recipe_comment.dart';
part 'model/recipe_content.dart';
part 'model/register_endpoint_request.dart';
part 'model/register_response.dart';
part 'model/substitution.dart';

/// An [ApiClient] instance that uses the default values obtained from
/// the OpenAPI specification file.
var defaultApiClient = ApiClient();

const _delimiters = {'csv': ',', 'ssv': ' ', 'tsv': '\t', 'pipes': '|'};
const _dateEpochMarker = 'epoch';
const _deepEquality = DeepCollectionEquality();
final _dateFormatter = DateFormat('yyyy-MM-dd');
final _regList = RegExp(r'^List<(.*)>$');
final _regSet = RegExp(r'^Set<(.*)>$');
final _regMap = RegExp(r'^Map<String,(.*)>$');

bool _isEpochMarker(String? pattern) =>
    pattern == _dateEpochMarker || pattern == '/$_dateEpochMarker/';
