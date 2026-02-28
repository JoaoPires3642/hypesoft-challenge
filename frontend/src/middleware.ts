import { auth } from "@/src/auth"

export default auth((req) => {
  const isLoggedIn = !!req.auth
  const isOnLoginPage = req.nextUrl.pathname === "/login"

  // Se não está logado e não está na página de login, redireciona para login
  if (!isLoggedIn && !isOnLoginPage) {
    return Response.redirect(new URL("/login", req.url))
  }

  // Se está logado e está na página de login, redireciona para home
  if (isLoggedIn && isOnLoginPage) {
    return Response.redirect(new URL("/", req.url))
  }
})

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico|.*\\.png$|.*\\.svg$).*)"],
}
