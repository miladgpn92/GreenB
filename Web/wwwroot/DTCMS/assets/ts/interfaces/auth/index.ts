export interface LoginModel {
  userName: string;
  userRole: string;
  passsword: string;
}
export interface TokenModel {
  access_token: string;
  expires_in: number;
  refresh_token: null;
  token_type: string;
}
export interface PassowrdModel {
  password: string;
  confirmPassword: string;
}
export interface CodeModel {
  userName: string;
  isForgotpass?: boolean;
}

export interface VerificationModel extends CodeModel {
  verificationCode: string;
}
