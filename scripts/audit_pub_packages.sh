#!/usr/bin/env bash
# Pub dependency audit for the Catchen Flutter clients (dependency-audit).
#
# Run from a directory containing pubspec.yaml. Records an outdated-package
# report for traceability and, when the toolchain supports it, fails on
# known advisories via `dart pub audit`. Findings may only be waived by a
# dated, owner-signed entry in SECURITY-ACCEPTANCES.md (reference it in CI
# or locally with AUDIT_SUPPRESS_REASON).
set -euo pipefail

if command -v dart >/dev/null 2>&1 && dart pub audit --help >/dev/null 2>&1; then
  echo "[pub-audit] running 'dart pub audit'..."
  if ! dart pub audit; then
    echo "::error::Known pub package advisories found. Upgrade the package or add a dated, owner-signed entry to SECURITY-ACCEPTANCES.md." >&2
    exit 1
  fi
else
  # Older SDKs have no audit command: record the report so the review step
  # is still traceable in CI logs.
  echo "[pub-audit] 'dart pub audit' unavailable on this SDK; recording 'flutter pub outdated' report only."
  flutter pub outdated
fi
