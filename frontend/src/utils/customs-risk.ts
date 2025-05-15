import { CustomsForm } from "@/model/shipment";

export function getCustomsRiskLevel(customs?: CustomsForm) {
  if (!customs) return { label: "Okänd", color: "text-gray-400", emoji: "❔" };
  // Plasmaaktiv + stabilitet < 5 + livsform
  if (
    customs.isPlasmaActive &&
    typeof customs.plasmaStabilityLevel === "number" &&
    customs.plasmaStabilityLevel < 5 &&
    customs.containsLifeforms
  ) {
    return { label: "Kritisk", color: "text-red-600", emoji: "🔴" };
  }
  // Livsform + okänd art
  if (
    customs.containsLifeforms &&
    customs.lifeformType &&
    ["okänd", "vet ej", "ej känd"].includes(customs.lifeformType.trim().toLowerCase())
  ) {
    return { label: "Hög", color: "text-orange-500", emoji: "🔶" };
  }
  // Plasmaaktiv med stabilitet 5–7
  if (
    customs.isPlasmaActive &&
    typeof customs.plasmaStabilityLevel === "number" &&
    customs.plasmaStabilityLevel >= 5 &&
    customs.plasmaStabilityLevel <= 7
  ) {
    return { label: "Medel", color: "text-yellow-500", emoji: "🟡" };
  }
  // Inga specialegenskaper
  return { label: "Låg", color: "text-green-600", emoji: "🟢" };
} 