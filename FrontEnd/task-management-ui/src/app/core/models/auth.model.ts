export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  roleId: number;
}

// Matches AuthResponseDto from the API exactly (camelCase - System.Text.Json's
// default naming policy).
export interface AuthResponse {
  token: string;
  expiresAtUtc: string;
  userId: number;
  fullName: string;
  email: string;
  role: string;
}
