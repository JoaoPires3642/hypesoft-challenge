"use client";

import { useSession } from "next-auth/react";
import { useRouter } from "next/navigation";
import { useEffect, type ReactNode } from "react";
import { Spinner } from "@/components/ui/spinner";

interface SessionGuardProps {
  children: ReactNode;
}

export function SessionGuard({ children }: SessionGuardProps) {
  const { data: session, status } = useSession();
  const router = useRouter();

  useEffect(() => {
    if (status === "unauthenticated") {
      router.push("/login");
    }
  }, [status, router]);

  if (status === "loading") {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="flex flex-col items-center gap-4">
          <Spinner className="h-8 w-8" />
          <p className="text-sm text-muted-foreground">
            Carregando sessão...
          </p>
        </div>
      </div>
    );
  }

  if (status === "unauthenticated") {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="flex flex-col items-center gap-4">
          <Spinner className="h-8 w-8" />
          <p className="text-sm text-muted-foreground">
            Redirecionando para login...
          </p>
        </div>
      </div>
    );
  }

  // Aguardar token estar disponível
  if (!session?.accessToken) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="flex flex-col items-center gap-4">
          <Spinner className="h-8 w-8" />
          <p className="text-sm text-muted-foreground">
            Autenticando...
          </p>
        </div>
      </div>
    );
  }

  return <>{children}</>;
}
