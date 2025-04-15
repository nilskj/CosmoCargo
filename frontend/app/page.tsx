"use client";

import SpaceBackground from "@/app/components/SpaceBackground";
import Header from "@/app/components/Header";
import Footer from "@/app/components/Footer";
import HeroSection from "@/app/components/HeroSection";
import FeatureList from "@/app/components/FeatureList";

export default function Home() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center">
      <SpaceBackground>
        <Header />
        <HeroSection />
        <FeatureList />
        <Footer />
      </SpaceBackground>
    </main>
  );
}
