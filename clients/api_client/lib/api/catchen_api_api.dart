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

  /// Performs an HTTP 'POST /api/admin/comments/{id}/hide' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  ///
  /// * [ReasonCodeRequest] reasonCodeRequest (required):
  Future<Response> apiAdminCommentsIdHidePostWithHttpInfo(
    String id,
    ReasonCodeRequest reasonCodeRequest,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/admin/comments/{id}/hide'.replaceAll('{id}', id);

    // ignore: prefer_final_locals
    Object? postBody = reasonCodeRequest;

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
  /// * [String] id (required):
  ///
  /// * [ReasonCodeRequest] reasonCodeRequest (required):
  Future<void> apiAdminCommentsIdHidePost(
    String id,
    ReasonCodeRequest reasonCodeRequest,
  ) async {
    final response = await apiAdminCommentsIdHidePostWithHttpInfo(
      id,
      reasonCodeRequest,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'POST /api/admin/drafts/{id}/publish' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  Future<Response> apiAdminDraftsIdPublishPostWithHttpInfo(
    String id,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/admin/drafts/{id}/publish'.replaceAll('{id}', id);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];

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
  /// * [String] id (required):
  Future<void> apiAdminDraftsIdPublishPost(
    String id,
  ) async {
    final response = await apiAdminDraftsIdPublishPostWithHttpInfo(
      id,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'POST /api/admin/drafts/{id}/secondary-review' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  Future<Response> apiAdminDraftsIdSecondaryReviewPostWithHttpInfo(
    String id,
  ) async {
    // ignore: prefer_const_declarations
    final path =
        r'/api/admin/drafts/{id}/secondary-review'.replaceAll('{id}', id);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];

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
  /// * [String] id (required):
  Future<void> apiAdminDraftsIdSecondaryReviewPost(
    String id,
  ) async {
    final response = await apiAdminDraftsIdSecondaryReviewPostWithHttpInfo(
      id,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

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

  /// Performs an HTTP 'POST /api/admin/recipes/{id}/unpublish' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  Future<Response> apiAdminRecipesIdUnpublishPostWithHttpInfo(
    String id,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/admin/recipes/{id}/unpublish'.replaceAll('{id}', id);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];

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
  /// * [String] id (required):
  Future<void> apiAdminRecipesIdUnpublishPost(
    String id,
  ) async {
    final response = await apiAdminRecipesIdUnpublishPostWithHttpInfo(
      id,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'POST /api/admin/users/{id}/block' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  ///
  /// * [ReasonCodeRequest] reasonCodeRequest (required):
  Future<Response> apiAdminUsersIdBlockPostWithHttpInfo(
    String id,
    ReasonCodeRequest reasonCodeRequest,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/admin/users/{id}/block'.replaceAll('{id}', id);

    // ignore: prefer_final_locals
    Object? postBody = reasonCodeRequest;

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
  /// * [String] id (required):
  ///
  /// * [ReasonCodeRequest] reasonCodeRequest (required):
  Future<void> apiAdminUsersIdBlockPost(
    String id,
    ReasonCodeRequest reasonCodeRequest,
  ) async {
    final response = await apiAdminUsersIdBlockPostWithHttpInfo(
      id,
      reasonCodeRequest,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
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

  /// Performs an HTTP 'GET /api/catalog/favorites' operation and returns the [Response].
  Future<Response> apiCatalogFavoritesGetWithHttpInfo() async {
    // ignore: prefer_const_declarations
    final path = r'/api/catalog/favorites';

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

  Future<CatalogListResponse?> apiCatalogFavoritesGet() async {
    final response = await apiCatalogFavoritesGetWithHttpInfo();
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
        'CatalogListResponse',
      ) as CatalogListResponse;
    }
    return null;
  }

  /// Performs an HTTP 'GET /api/catalog/recipes' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] category:
  ///
  /// * [String] difficulty:
  ///
  /// * [String] ingredient:
  ///
  /// * [String] q:
  Future<Response> apiCatalogRecipesGetWithHttpInfo({
    String? category,
    String? difficulty,
    String? ingredient,
    String? q,
  }) async {
    // ignore: prefer_const_declarations
    final path = r'/api/catalog/recipes';

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    if (category != null) {
      queryParams.addAll(_queryParams('', 'category', category));
    }
    if (difficulty != null) {
      queryParams.addAll(_queryParams('', 'difficulty', difficulty));
    }
    if (ingredient != null) {
      queryParams.addAll(_queryParams('', 'ingredient', ingredient));
    }
    if (q != null) {
      queryParams.addAll(_queryParams('', 'q', q));
    }

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

  /// Parameters:
  ///
  /// * [String] category:
  ///
  /// * [String] difficulty:
  ///
  /// * [String] ingredient:
  ///
  /// * [String] q:
  Future<CatalogListResponse?> apiCatalogRecipesGet({
    String? category,
    String? difficulty,
    String? ingredient,
    String? q,
  }) async {
    final response = await apiCatalogRecipesGetWithHttpInfo(
      category: category,
      difficulty: difficulty,
      ingredient: ingredient,
      q: q,
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
        'CatalogListResponse',
      ) as CatalogListResponse;
    }
    return null;
  }

  /// Performs an HTTP 'GET /api/catalog/recipes/{id}/comments' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  Future<Response> apiCatalogRecipesIdCommentsGetWithHttpInfo(
    String id,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/catalog/recipes/{id}/comments'.replaceAll('{id}', id);

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

  /// Parameters:
  ///
  /// * [String] id (required):
  Future<CommentListResponse?> apiCatalogRecipesIdCommentsGet(
    String id,
  ) async {
    final response = await apiCatalogRecipesIdCommentsGetWithHttpInfo(
      id,
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
        'CommentListResponse',
      ) as CommentListResponse;
    }
    return null;
  }

  /// Performs an HTTP 'POST /api/catalog/recipes/{id}/comments' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  ///
  /// * [CommentRequest] commentRequest (required):
  Future<Response> apiCatalogRecipesIdCommentsPostWithHttpInfo(
    String id,
    CommentRequest commentRequest,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/catalog/recipes/{id}/comments'.replaceAll('{id}', id);

    // ignore: prefer_final_locals
    Object? postBody = commentRequest;

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
  /// * [String] id (required):
  ///
  /// * [CommentRequest] commentRequest (required):
  Future<CommentCreatedResponse?> apiCatalogRecipesIdCommentsPost(
    String id,
    CommentRequest commentRequest,
  ) async {
    final response = await apiCatalogRecipesIdCommentsPostWithHttpInfo(
      id,
      commentRequest,
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
        'CommentCreatedResponse',
      ) as CommentCreatedResponse;
    }
    return null;
  }

  /// Performs an HTTP 'DELETE /api/catalog/recipes/{id}/favorite' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  Future<Response> apiCatalogRecipesIdFavoriteDeleteWithHttpInfo(
    String id,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/catalog/recipes/{id}/favorite'.replaceAll('{id}', id);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];

    return apiClient.invokeAPI(
      path,
      'DELETE',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] id (required):
  Future<void> apiCatalogRecipesIdFavoriteDelete(
    String id,
  ) async {
    final response = await apiCatalogRecipesIdFavoriteDeleteWithHttpInfo(
      id,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'POST /api/catalog/recipes/{id}/favorite' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  Future<Response> apiCatalogRecipesIdFavoritePostWithHttpInfo(
    String id,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/catalog/recipes/{id}/favorite'.replaceAll('{id}', id);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];

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
  /// * [String] id (required):
  Future<void> apiCatalogRecipesIdFavoritePost(
    String id,
  ) async {
    final response = await apiCatalogRecipesIdFavoritePostWithHttpInfo(
      id,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'GET /api/catalog/recipes/{id}' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  Future<Response> apiCatalogRecipesIdGetWithHttpInfo(
    String id,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/catalog/recipes/{id}'.replaceAll('{id}', id);

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

  /// Parameters:
  ///
  /// * [String] id (required):
  Future<CatalogDetail?> apiCatalogRecipesIdGet(
    String id,
  ) async {
    final response = await apiCatalogRecipesIdGetWithHttpInfo(
      id,
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
        'CatalogDetail',
      ) as CatalogDetail;
    }
    return null;
  }

  /// Performs an HTTP 'PUT /api/editorial/drafts/{id}' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  ///
  /// * [CreateDraftRequest] createDraftRequest (required):
  Future<Response> apiEditorialDraftsIdPutWithHttpInfo(
    String id,
    CreateDraftRequest createDraftRequest,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/editorial/drafts/{id}'.replaceAll('{id}', id);

    // ignore: prefer_final_locals
    Object? postBody = createDraftRequest;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>['application/json'];

    return apiClient.invokeAPI(
      path,
      'PUT',
      queryParams,
      postBody,
      headerParams,
      formParams,
      contentTypes.isEmpty ? null : contentTypes.first,
    );
  }

  /// Parameters:
  ///
  /// * [String] id (required):
  ///
  /// * [CreateDraftRequest] createDraftRequest (required):
  Future<void> apiEditorialDraftsIdPut(
    String id,
    CreateDraftRequest createDraftRequest,
  ) async {
    final response = await apiEditorialDraftsIdPutWithHttpInfo(
      id,
      createDraftRequest,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'POST /api/editorial/drafts/{id}/submit' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [String] id (required):
  Future<Response> apiEditorialDraftsIdSubmitPostWithHttpInfo(
    String id,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/editorial/drafts/{id}/submit'.replaceAll('{id}', id);

    // ignore: prefer_final_locals
    Object? postBody;

    final queryParams = <QueryParam>[];
    final headerParams = <String, String>{};
    final formParams = <String, String>{};

    const contentTypes = <String>[];

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
  /// * [String] id (required):
  Future<void> apiEditorialDraftsIdSubmitPost(
    String id,
  ) async {
    final response = await apiEditorialDraftsIdSubmitPostWithHttpInfo(
      id,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
  }

  /// Performs an HTTP 'POST /api/editorial/drafts' operation and returns the [Response].
  /// Parameters:
  ///
  /// * [CreateDraftRequest] createDraftRequest (required):
  Future<Response> apiEditorialDraftsPostWithHttpInfo(
    CreateDraftRequest createDraftRequest,
  ) async {
    // ignore: prefer_const_declarations
    final path = r'/api/editorial/drafts';

    // ignore: prefer_final_locals
    Object? postBody = createDraftRequest;

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
  /// * [CreateDraftRequest] createDraftRequest (required):
  Future<void> apiEditorialDraftsPost(
    CreateDraftRequest createDraftRequest,
  ) async {
    final response = await apiEditorialDraftsPostWithHttpInfo(
      createDraftRequest,
    );
    if (response.statusCode >= HttpStatus.badRequest) {
      throw ApiException(response.statusCode, await _decodeBodyBytes(response));
    }
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
