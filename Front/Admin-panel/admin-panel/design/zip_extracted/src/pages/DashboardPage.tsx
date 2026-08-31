import SideBadge, { StatusBadge } from "../components/SideBadge";
import { recentActivity } from "../data/mockData";

interface StatCardProps {
  label: string;
  value: number;
  sub?: string;
  accent?: boolean;
}

function StatCard({ label, value, sub, accent }: StatCardProps) {
  return (
    <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5 flex flex-col gap-3 glow-red-hover transition-all duration-200 group cursor-default">
      <span className="text-[#9CA3AF] text-xs font-medium tracking-wider uppercase">{label}</span>
      <div className="flex items-end gap-2">
        <span
          className={`text-4xl font-bold tabular-nums tracking-tight transition-all duration-200
            ${accent ? "text-[#DC2626] group-hover:drop-shadow-[0_0_12px_rgba(220,38,38,0.5)]" : "text-white"}`}
        >
          {value.toLocaleString()}
        </span>
      </div>
      {sub && <span className="text-[#9CA3AF] text-[11px] font-mono">{sub}</span>}
    </div>
  );
}

function formatTime(iso: string) {
  const d = new Date(iso);
  return d.toLocaleString("en-US", { month: "short", day: "numeric", hour: "2-digit", minute: "2-digit" });
}

export default function DashboardPage() {
  return (
    <div className="animate-fade-in space-y-6">
      {/* Stats */}
      <div className="grid grid-cols-4 gap-4">
        <StatCard label="Pending Strats" value={23} sub="Awaiting review" accent />
        <StatCard label="Pending Categories" value={7} sub="Awaiting review" accent />
        <StatCard label="Total Strats" value={412} sub="All time published" />
        <StatCard label="Total Categories" value={48} sub="Active categories" />
      </div>

      {/* Quick actions */}
      <div className="flex items-center gap-3">
        <span className="text-[#9CA3AF] text-xs font-mono tracking-wider uppercase">Quick Actions</span>
        <div className="flex-1 h-px bg-[#2A2A2E]" />
        <button className="flex items-center gap-2 px-4 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-sm font-semibold rounded transition-colors">
          <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
          </svg>
          Approve Selected
        </button>
        <button className="flex items-center gap-2 px-4 py-2 bg-[#1C1C1F] hover:bg-[#242428] text-[#EF4444] text-sm font-semibold rounded border border-[#DC2626] transition-colors">
          <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
          </svg>
          Reject Selected
        </button>
      </div>

      {/* Recent Activity */}
      <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded overflow-hidden">
        <div className="px-5 py-3.5 border-b border-[#2A2A2E] flex items-center justify-between">
          <h2 className="text-white font-semibold text-sm">Recent Activity</h2>
          <span className="text-[#9CA3AF] text-[11px] font-mono">{recentActivity.length} entries</span>
        </div>
        <table className="w-full">
          <thead>
            <tr className="border-b border-[#2A2A2E]">
              {["ID", "Name", "Type", "Side", "Map", "Submitted By", "Date", "Status"].map((h) => (
                <th key={h} className="text-left text-[10px] text-[#9CA3AF] font-mono tracking-widest uppercase px-5 py-2.5">
                  {h}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {recentActivity.map((row, i) => (
              <tr
                key={row.id}
                className={`table-row-hover border-b border-[#2A2A2E] last:border-b-0 transition-colors ${i % 2 === 0 ? "" : "bg-[rgba(255,255,255,0.01)]"}`}
              >
                <td className="px-5 py-3.5">
                  <span className="text-[#9CA3AF] text-xs font-mono">{row.id}</span>
                </td>
                <td className="px-5 py-3.5">
                  <span className="text-white text-sm font-medium">{row.name}</span>
                </td>
                <td className="px-5 py-3.5">
                  <span className="text-[#9CA3AF] text-xs font-mono">{row.type}</span>
                </td>
                <td className="px-5 py-3.5">
                  <SideBadge side={row.side} />
                </td>
                <td className="px-5 py-3.5">
                  <span className="text-[#9CA3AF] text-sm">{row.map}</span>
                </td>
                <td className="px-5 py-3.5">
                  <span className="text-white text-sm">{row.submittedBy}</span>
                </td>
                <td className="px-5 py-3.5">
                  <span className="text-[#9CA3AF] text-xs font-mono whitespace-nowrap">{formatTime(row.submittedAt)}</span>
                </td>
                <td className="px-5 py-3.5">
                  <StatusBadge status={row.status} />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* System status */}
      <div className="grid grid-cols-3 gap-4">
        {[
          { label: "API Status", value: "Operational", ok: true },
          { label: "Database", value: "Healthy", ok: true },
          { label: "Last Sync", value: "2 min ago", ok: true },
        ].map((s) => (
          <div key={s.label} className="bg-[#1C1C1F] border border-[#2A2A2E] rounded px-4 py-3 flex items-center justify-between">
            <span className="text-[#9CA3AF] text-xs font-mono uppercase tracking-wider">{s.label}</span>
            <div className="flex items-center gap-2">
              <div className={`w-1.5 h-1.5 rounded-full ${s.ok ? "bg-[#16A34A]" : "bg-[#DC2626]"}`} />
              <span className={`text-xs font-medium ${s.ok ? "text-[#4ADE80]" : "text-[#EF4444]"}`}>{s.value}</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
