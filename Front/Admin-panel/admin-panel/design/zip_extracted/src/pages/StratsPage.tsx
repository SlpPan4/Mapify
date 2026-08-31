import { useState } from "react";
import type { Strat } from "../data/mockData";
import { mockStrats } from "../data/mockData";
import SideBadge from "../components/SideBadge";
import CreateEditModal from "../components/CreateEditModal";

interface FormData {
  name: string;
  side: import("../data/mockData").Side;
  map: string;
  categories: string[];
  operators: string[];
  description: string;
  videoUrl: string;
}

interface Props {
  onToast: (msg: string, type?: "success" | "error") => void;
  onView: (strat: Strat) => void;
}

export default function StratsPage({ onToast, onView }: Props) {
  const [strats, setStrats] = useState<Strat[]>(mockStrats);
  const [search, setSearch] = useState("");
  const [sideFilter, setSideFilter] = useState<"all" | "Attack" | "Defense">("all");
  const [modal, setModal] = useState<{ open: boolean; mode: "create" | "edit"; strat?: Strat }>({ open: false, mode: "create" });

  const rows = strats.filter((s) => {
    if (sideFilter !== "all" && s.side !== sideFilter) return false;
    if (search && !s.name.toLowerCase().includes(search.toLowerCase())) return false;
    return true;
  });

  function handleDelete(id: string) {
    setStrats((prev) => prev.filter((s) => s.id !== id));
    onToast("Strat deleted successfully", "success");
  }

  function handleSave(data: FormData) {
    if (modal.mode === "create") {
      const newStrat: Strat = {
        id: `STR-${String(strats.length + 1).padStart(3, "0")}`,
        createdAt: new Date().toISOString(),
        name: data.name,
        side: data.side,
        map: data.map,
        categories: data.categories,
        operators: data.operators,
        description: data.description,
        videoUrl: data.videoUrl,
      };
      setStrats((prev) => [newStrat, ...prev]);
      onToast("Strat created successfully", "success");
    } else if (modal.strat) {
      setStrats((prev) => prev.map((s) => s.id === modal.strat!.id ? { ...s, ...data } : s));
      onToast("Strat updated successfully", "success");
    }
  }

  return (
    <div className="animate-fade-in space-y-5">
      {/* Toolbar */}
      <div className="flex items-center gap-3 flex-wrap">
        <div className="relative flex-1 min-w-[180px] max-w-xs">
          <svg className="w-3.5 h-3.5 text-[#9CA3AF] absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search strats…"
            className="w-full bg-[#1C1C1F] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] pl-8 pr-3 py-2 focus:outline-none focus:border-[#DC2626] transition-colors"
          />
        </div>
        <div className="flex items-center gap-1 bg-[#1C1C1F] border border-[#2A2A2E] rounded p-1">
          {(["all", "Attack", "Defense"] as const).map((f) => (
            <button key={f} onClick={() => setSideFilter(f)}
              className={`px-3 py-1 text-xs font-medium rounded transition-colors ${sideFilter === f ? "bg-[#DC2626] text-white" : "text-[#9CA3AF] hover:text-white"}`}>
              {f === "all" ? "All" : f}
            </button>
          ))}
        </div>
        <div className="flex-1" />
        <button
          onClick={() => setModal({ open: true, mode: "create" })}
          className="flex items-center gap-2 px-4 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-sm font-semibold rounded transition-colors"
        >
          <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4v16m8-8H4" />
          </svg>
          Add New Strat
        </button>
      </div>

      {/* Table */}
      <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded overflow-hidden">
        {rows.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-16 gap-3">
            <svg className="w-10 h-10 text-[#2A2A2E]" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7" />
            </svg>
            <p className="text-[#9CA3AF] text-sm">No strats found</p>
          </div>
        ) : (
          <table className="w-full">
            <thead>
              <tr className="border-b border-[#2A2A2E]">
                {["ID", "Name", "Side", "Map", "Categories", "Operators", "Created", "Actions"].map((h) => (
                  <th key={h} className="text-left text-[10px] text-[#9CA3AF] font-mono tracking-widest uppercase px-5 py-2.5 whitespace-nowrap">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {rows.map((row, i) => (
                <tr key={row.id} className={`table-row-hover border-b border-[#2A2A2E] last:border-b-0 transition-colors ${i % 2 !== 0 ? "bg-[rgba(255,255,255,0.01)]" : ""}`}>
                  <td className="px-5 py-3.5"><span className="text-[#9CA3AF] text-xs font-mono">{row.id}</span></td>
                  <td className="px-5 py-3.5"><span className="text-white text-sm font-medium">{row.name}</span></td>
                  <td className="px-5 py-3.5"><SideBadge side={row.side} /></td>
                  <td className="px-5 py-3.5"><span className="text-[#9CA3AF] text-sm">{row.map}</span></td>
                  <td className="px-5 py-3.5">
                    <div className="flex flex-wrap gap-1">
                      {row.categories.slice(0, 2).map((c) => (
                        <span key={c} className="px-1.5 py-0.5 bg-[#141416] border border-[#2A2A2E] rounded text-[10px] text-[#9CA3AF] font-mono">{c}</span>
                      ))}
                      {row.categories.length > 2 && <span className="text-[#9CA3AF] text-xs">+{row.categories.length - 2}</span>}
                    </div>
                  </td>
                  <td className="px-5 py-3.5">
                    <div className="flex flex-wrap gap-1">
                      {row.operators.slice(0, 2).map((op) => (
                        <span key={op} className="px-1.5 py-0.5 bg-[#141416] border border-[#2A2A2E] rounded text-[10px] text-white font-mono">{op}</span>
                      ))}
                      {row.operators.length > 2 && <span className="text-[#9CA3AF] text-xs">+{row.operators.length - 2}</span>}
                    </div>
                  </td>
                  <td className="px-5 py-3.5">
                    <span className="text-[#9CA3AF] text-xs font-mono whitespace-nowrap">
                      {new Date(row.createdAt).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })}
                    </span>
                  </td>
                  <td className="px-5 py-3.5">
                    <div className="flex items-center gap-1.5">
                      <button
                        onClick={() => onView(row)}
                        className="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#141416] border border-transparent hover:border-[#2A2A2E] transition-all"
                        title="View"
                      >
                        <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                          <path strokeLinecap="round" strokeLinejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                        </svg>
                      </button>
                      <button
                        onClick={() => setModal({ open: true, mode: "edit", strat: row })}
                        className="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#141416] border border-transparent hover:border-[#2A2A2E] transition-all"
                        title="Edit"
                      >
                        <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                        </svg>
                      </button>
                      <button
                        onClick={() => handleDelete(row.id)}
                        className="w-7 h-7 flex items-center justify-center rounded text-[#DC2626] hover:text-white hover:bg-[#DC2626] border border-transparent hover:border-[#DC2626] transition-all"
                        title="Delete"
                      >
                        <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                        </svg>
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <CreateEditModal
        open={modal.open}
        mode={modal.mode}
        type="strat"
        initial={modal.strat ? {
          id: modal.strat.id,
          name: modal.strat.name,
          side: modal.strat.side,
          map: modal.strat.map,
          categories: modal.strat.categories,
          operators: modal.strat.operators,
          description: modal.strat.description,
          videoUrl: modal.strat.videoUrl,
        } : undefined}
        onClose={() => setModal((p) => ({ ...p, open: false }))}
        onSave={handleSave}
      />
    </div>
  );
}
