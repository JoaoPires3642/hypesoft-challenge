import NextAuth from "next-auth"
import Keycloak from "next-auth/providers/keycloak"

// Keycloak configuration - must be set via environment variables
// KEYCLOAK_ISSUER: URL of the Keycloak realm (e.g., http://localhost:8080/realms/hypesoft-realm)
const KEYCLOAK_ISSUER = process.env.KEYCLOAK_ISSUER || "http://localhost:8080/realms/hypesoft-realm"
const KEYCLOAK_CLIENT_ID = process.env.KEYCLOAK_CLIENT_ID || "hypesoft-client"
const KEYCLOAK_CLIENT_SECRET = process.env.KEYCLOAK_CLIENT_SECRET || ""

if (!process.env.KEYCLOAK_ISSUER) {
  console.warn('KEYCLOAK_ISSUER not set, using default:', KEYCLOAK_ISSUER);
}
if (!process.env.KEYCLOAK_CLIENT_SECRET) {
  console.warn('KEYCLOAK_CLIENT_SECRET not set. Authentication may fail.');
}

export const { handlers, signIn, signOut, auth } = NextAuth({
  providers: [
    Keycloak({
      clientId: KEYCLOAK_CLIENT_ID,
      clientSecret: KEYCLOAK_CLIENT_SECRET,
      issuer: KEYCLOAK_ISSUER,
      authorization: {
        params: {
          scope: "openid email profile",
        },
      },
    }),
  ],
  trustHost: true,
  callbacks: {
    async jwt({ token, account }) {
      // Persist the OAuth access_token to the token right after signin
      if (account) {
        token.accessToken = account.access_token
        token.idToken = account.id_token
        token.refreshToken = account.refresh_token
        token.expiresAt = account.expires_at
      }
      return token
    },
    async session({ session, token }) {
      // Send properties to the client
      session.accessToken = token.accessToken as string
      session.idToken = token.idToken as string
      session.error = token.error as string | undefined
      return session
    },
  },
  pages: {
    signIn: "/login",
  },
  session: {
    strategy: "jwt",
  },
})
