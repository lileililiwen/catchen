//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//
// @dart=2.18

// ignore_for_file: unused_element, unused_import
// ignore_for_file: always_put_required_named_parameters_first
// ignore_for_file: constant_identifier_names
// ignore_for_file: lines_longer_than_80_chars

part of openapi.api;

class CatchenApiApi {
  CatchenApiApi([ApiClient? apiClient])
      : apiClient = apiClient ?? defaultApiClient;

  final ApiClient apiClient;

  /// Performs an HTTP 'GET /api/admin/promotion-channels/approvals' operation and returns the [Response].
  Future<Response> apiAdminPromotionChannelsApprovalsGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/api/admin/promotion-channels/approvals';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];

    return apiClient.invokeAPI(
      path,
      'GET',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  Future<ApprovedChannelsResponse?>
      apiAdminPromotionChannelsApprovalsGet() async {
    final response = await apiAdminPromotionChannelsApprovalsGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty &&
        response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(
        await _decodeBodyBytes(response),
        'ApprovedChannelsResponse',
      ) as ApprovedChannelsResponse;
    }
    return null;
  }

  /// Performs an HTTP 'POST /api/admin/promotion-channels/approvals' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [ApproveChannelRequest] approveChannelRequest (required):
  Future<Response> apiAdminPromotionChannelsApprovalsPostWithHttpInfo(
    ApproveChannelRequest approveChannelRequest,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/admin/promotion-channels/approvals';

    // ignore: prefer_final_locals
    Object? postBody = approveChannelRequest;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>['application/json'];

    return apiClient.invokeAPI(
      path,
      'POST',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [ApproveChannelRequest] approveChannelRequest (required):
  Future<ApprovalResponse?> apiAdminPromotionChannelsApprovalsPost(
    ApproveChannelRequest approveChannelRequest,
  ) async {
    final response = await apiAdminPromotionChannelsApprovalsPostWithHttpInfo(
      approveChannelRequest,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty &&
        response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(
        await _decodeBodyBytes(response),
        'ApprovalResponse',
      ) as ApprovalResponse;
    }
    return null;
  }

  /// Performs an HTTP 'POST /api/auth/login' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [LoginEndpointRequest] loginEndpointRequest (required):
  Future<Response> apiAuthLoginPostWithHttpInfo(
    LoginEndpointRequest loginEndpointRequest,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/auth/login';

    // ignore: prefer_final_locals
    Object? postBody = loginEndpointRequest;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>['application/json'];

    return apiClient.invokeAPI(
      path,
      'POST',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [LoginEndpointRequest] loginEndpointRequest (required):
  Future<LoginResponse?> apiAuthLoginPost(
    LoginEndpointRequest loginEndpointRequest,
  ) async {
    final response = await apiAuthLoginPostWithHttpInfo(
      loginEndpointRequest,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty &&
        response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(
        await _decodeBodyBytes(response),
        'LoginResponse',
      ) as LoginResponse;
    }
    return null;
  }

  /// Performs an HTTP 'POST /api/auth/register' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [RegisterEndpointRequest] registerEndpointRequest (required):
  Future<Response> apiAuthRegisterPostWithHttpInfo(
    RegisterEndpointRequest registerEndpointRequest,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/auth/register';

    // ignore: prefer_final_locals
    Object? postBody = registerEndpointRequest;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>['application/json'];

    return apiClient.invokeAPI(
      path,
      'POST',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [RegisterEndpointRequest] registerEndpointRequest (required):
  Future<RegisterResponse?> apiAuthRegisterPost(
    RegisterEndpointRequest registerEndpointRequest,
  ) async {
    final response = await apiAuthRegisterPostWithHttpInfo(
      registerEndpointRequest,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty &&
        response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(
        await _decodeBodyBytes(response),
        'RegisterResponse',
      ) as RegisterResponse;
    }
    return null;
  }

  /// Performs an HTTP 'GET /api/policy/payment-methods' operation and returns the [Response].
  Future<Response> apiPolicyPaymentMethodsGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/api/policy/payment-methods';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];

    return apiClient.invokeAPI(
      path,
      'GET',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  Future<PaymentMethodsResponse?> apiPolicyPaymentMethodsGet() async {
    final response = await apiPolicyPaymentMethodsGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
    // When a remote server returns no body with a status of 204, we shall not decode it.
    // At the time of writing this, `dart:convert` will throw an "Unexpected end of input"
    // FormatException when trying to decode an empty string.
    if (response.body.isNotEmpty &&
        response.statusCode != HttpStatus.noContent) {
      return await apiClient.deserializeAsync(
        await _decodeBodyBytes(response),
        'PaymentMethodsResponse',
      ) as PaymentMethodsResponse;
    }
    return null;
  }

  /// Performs an HTTP 'GET /healthz' operation and returns the [Response].
  Future<Response> healthzGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/healthz';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];

    return apiClient.invokeAPI(
      path,
      'GET',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  Future<void> healthzGet() async {
    final response = await healthzGetWithHttpInfo();
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }
}
