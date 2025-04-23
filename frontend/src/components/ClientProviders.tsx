"use client";

import React from "react";
import { Toaster } from "sonner";
import dynamic from "next/dynamic";
import { AuthProvider } from "../contexts/AuthContext";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
// Dynamically import BrowserRouter with ssr disabled
const BrowserRouterWrapper = dynamic(
  () =>
    import("./BrowserRouterWrapper").then((mod) => mod.BrowserRouterWrapper),
  { ssr: false }
);

const queryClient = new QueryClient();

export default function ClientProviders({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <BrowserRouterWrapper>
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          {children}
          <Toaster />
        </AuthProvider>
      </QueryClientProvider>
    </BrowserRouterWrapper>
  );
}
