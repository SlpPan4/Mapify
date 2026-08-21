import type { Side } from "../data/mockData";

export default function SideBadge({ side }: { side: Side }) {
  return (
    <span
      className={`inline-flex items-center px-2 py-0.5 rounded text-[11px] font-semibold font-mono tracking-wide
        ${side === "Attack"
          ? "bg-[rgba(220,38,38,0.15)] text-[#EF4444] border border-[rgba(220,38,38,0.25)]"
          : "bg-[rgba(37,99,235,0.15)] text-[#60A5FA] border border-[rgba(37,99,235,0.25)]"
        }`}
    >
      {side === "Attack" ? "⚔" : "🛡"} {side.toUpperCase()}
    </span>
  );
}

export function StatusBadge({ status }: { status: string }) {
  const map: Record<string, { bg: string; text: string; label: string }> = {
    pending: { bg: "bg-[rgba(234,179,8,0.12)] border border-[rgba(234,179,8,0.25)]", text: "text-yellow-400", label: "PENDING" },
    approved: { bg: "bg-[rgba(22,163,74,0.12)] border border-[rgba(22,163,74,0.25)]", text: "text-[#4ADE80]", label: "APPROVED" },
    rejected: { bg: "bg-[rgba(220,38,38,0.12)] border border-[rgba(220,38,38,0.25)]", text: "text-[#EF4444]", label: "REJECTED" },
  };
  const s = map[status] ?? map.pending;
  return (
    <span className={`inline-flex items-center px-2 py-0.5 rounded text-[10px] font-bold font-mono tracking-wider ${s.bg} ${s.text}`}>
      {s.label}
    </span>
  );
}
