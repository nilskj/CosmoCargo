"use client";

import React from "react";
import { Toaster } from "sonner";
import dynamic from "next/dynamic";
import { AuthProvider } from "@/contexts/AuthContext";

// Dynamically import BrowserRouter with ssr disabled
const BrowserRouterWrapper = dynamic(
  () =>
    import("./BrowserRouterWrapper").then((mod) => mod.BrowserRouterWrapper),
  { ssr: false }
);

export default function ClientProviders({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <BrowserRouterWrapper>
      <AuthProvider>
        {children}
        <Toaster />
      </AuthProvider>
    </BrowserRouterWrapper>
  );
}
