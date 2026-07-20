// Matches UserDto from the /api/users endpoint.
export interface AppUser {
  userId: number;
  fullName: string;
  email: string;
  roleName: string;
}
