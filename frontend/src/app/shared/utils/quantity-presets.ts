export interface QuantityHistoryItem {
  id: string | number;
  cantidad: number;
}

export interface QuantityHistoryOrder {
  items: QuantityHistoryItem[];
}

export interface QuantityPreset {
  label: string;
  value: number;
  source?: 'history' | 'default';
}

type QuantityUnitKind = 'weight-kg' | 'weight-g' | 'volume-l' | 'volume-ml' | 'unit';

const UNIT_ALIASES = {
  weightKg: ['KG', 'KILO', 'KILOS'],
  weightG: ['G', 'GR', 'GRAMO', 'GRAMOS'],
  volumeL: ['L', 'LT', 'LITRO', 'LITROS'],
  volumeMl: ['ML', 'MILILITRO', 'MILILITROS'],
};

export function buildSmartQuantityPresets(
  history: QuantityHistoryOrder[],
  productId: string | number | null | undefined,
  unitName: string,
  fallbackPresets: QuantityPreset[],
  maxPresets = 5
): QuantityPreset[] {
  if (productId === null || productId === undefined) return fallbackPresets.slice(0, maxPresets);

  const unitKind = getQuantityUnitKind(unitName);
  const frequencies = new Map<number, number>();

  for (const order of history) {
    for (const item of order.items) {
      if (item.id.toString() !== productId.toString() || item.cantidad <= 0) continue;
      const displayValue = normalizePresetValue(toDisplayQuantity(item.cantidad, unitKind), unitKind);
      if (displayValue <= 0) continue;
      frequencies.set(displayValue, (frequencies.get(displayValue) ?? 0) + 1);
    }
  }

  const historicalPresets = [...frequencies.entries()]
    .sort(([valueA, countA], [valueB, countB]) => countB - countA || valueB - valueA)
    .map(([value]) => ({ label: formatQuantityPresetLabel(value, unitKind), value, source: 'history' as const }));

  const defaultPresets = fallbackPresets.map(preset => ({
    ...preset,
    source: preset.source ?? 'default' as const,
  }));
  const merged = [...historicalPresets, ...defaultPresets];
  const seen = new Set<number>();

  return merged
    .filter(preset => {
      if (seen.has(preset.value)) return false;
      seen.add(preset.value);
      return true;
    })
    .slice(0, maxPresets);
}

function getQuantityUnitKind(unitName: string): QuantityUnitKind {
  const unit = unitName.trim().toUpperCase();
  if (UNIT_ALIASES.weightKg.includes(unit)) return 'weight-kg';
  if (UNIT_ALIASES.weightG.includes(unit)) return 'weight-g';
  if (UNIT_ALIASES.volumeL.includes(unit)) return 'volume-l';
  if (UNIT_ALIASES.volumeMl.includes(unit)) return 'volume-ml';
  return 'unit';
}

function toDisplayQuantity(quantity: number, unitKind: QuantityUnitKind): number {
  if (unitKind === 'weight-kg' || unitKind === 'volume-l') return quantity * 1000;
  return quantity;
}

function normalizePresetValue(value: number, unitKind: QuantityUnitKind): number {
  if (unitKind === 'weight-kg' || unitKind === 'weight-g' || unitKind === 'volume-l' || unitKind === 'volume-ml') {
    return Math.round(value);
  }

  return Number(value.toFixed(2));
}

function formatQuantityPresetLabel(value: number, unitKind: QuantityUnitKind): string {
  if (unitKind === 'weight-kg' || unitKind === 'weight-g') {
    return value >= 1000 && value % 1000 === 0 ? `${value / 1000} kg` : `${value} g`;
  }

  if (unitKind === 'volume-l' || unitKind === 'volume-ml') {
    return value >= 1000 && value % 1000 === 0 ? `${value / 1000} l` : `${value} ml`;
  }

  return `${value}`;
}
