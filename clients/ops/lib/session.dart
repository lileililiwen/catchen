import 'dart:convert';

/// Authenticated operator session (in-memory; durable storage lands with the
/// verified-identity follow-up).
class Session {
  Session({required this.token, required this.role, required this.email});

  final String token;
  final String role;
  final String email;

  static Session? current;

  static const _adminRole = 'Administrator';

  bool get isAdministrator => role == _adminRole;

  static Session? fromToken(String token) {
    final parts = token.split('.');
    if (parts.length != 3) {
      return null;
    }

    try {
      final payload = utf8.decode(
        base64Url.decode(base64Url.normalize(parts[1])),
      );
      final claims = jsonDecode(payload) as Map<String, dynamic>;
      final role = _firstOf(claims, const [
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
        'role',
      ]);
      final email = _firstOf(claims, const [
        'email',
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress',
      ]);
      if (role is! String || role.isEmpty) {
        return null;
      }
      return Session(token: token, role: role, email: email?.toString() ?? '');
    } on FormatException {
      return null;
    }
  }

  static void signOut() => current = null;

  static dynamic _firstOf(Map<String, dynamic> claims, List<String> keys) {
    for (final key in keys) {
      final value = claims[key];
      if (value != null) {
        return value;
      }
    }
    return null;
  }
}
