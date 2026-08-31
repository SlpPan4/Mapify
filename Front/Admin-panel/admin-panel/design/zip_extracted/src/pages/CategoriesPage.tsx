import { useState } from "react";
import type { Category } from "../data/mockData";
import { mockCategories } from "../data/mockData";
import SideBadge from "../components/SideBadge";
import CreateEditModal from "../components/CreateEditModal";

interface Props {
  onToast: (msg: string, type?: "success" | "error") => void;
}

export default function CategoriesPage({ onToast }: Props) {
  const [cats, setCats] = useState<Category[]>(mockCategories);
  const [search, setSearch] = useState("");
  const [sideFilter, setSideFilter] = useState<"all" | "Attack" | "Defense">("all");
  const [modal, setModal] = useState<{ open: boolean; mode: "create" | "edit"; cat?: Category }>({ open: false, mode: "create" });

  const rows = cats.filter((c) => {
    if (sideFilter !== "all" && c.side !== sideFilter) return false;
    if (search && !c.name.toLowerCase().includes(search.toLowerCase())) return false;
    return true;
  });

  function handleDelete(id: string) {
    setCats((prev) => prev.filter((c) => c.id !== id));
    onToast("Category deleted", "success");
  }

  function handleSave(data: { name: string; side: import("../data/mockData").Side; description: string }) {
    if (modal.mode === "create") {
      const newCat: Category = {
        id: `CAT-${String(cats.length + 1).padStart(3, "0")}`,
        name: data.name,
        side: data.side,
        description: data.description,
        stratCount: 0,
        createdAt: new Date().toISOString(),
      };
      setCats((prev) => [newCat, ...prev]);
      onToast("Category created", "success");
    } else if (modal.cat) {
      setCats((prev) => prev.map((c) => c.id === modal.cat!.id ? { ...c, name: data.name, side: data.side, description: data.description } : c));
      onToast("Category updated", "success");
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
            placeholder="Search categories…"
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
          Add Category
        </button>
      </div>

      {/* Table */}
      <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded overflow-hidden">
        <table className="w-full">
          <thead>
            <tr className="border-b border-[#2A2A2E]">
              {["ID", "Name", "Side", "Description", "Strats", "Created", "Actions"].map((h) => (
                <th key={h} className="text-left text-[10px] text-[#9CA3AF] font-mono tracking-widest uppercase px-5 py-2.5 whitespace-nowrap">{h}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {rows.map((row, i) => (
              <tr key={row.id} className={`table-row-hover border-b border-[#2A2A2E] last:border-b-0 transition-colors ${i % 2 !== 0 ? "bg-[rgba(255,255,255,0.01)]" : ""}`}>
                <td className="px-5 py-3.5"><span className="text-[#9CA3AF] text-xs font-mono">{row.id}</span></td>
                <td className="px-5 py-3.5"><span className="text-white text-sm font-semibold">{row.name}</span></td>
                <td className="px-5 py-3.5"><SideBadge side={row.side} /></td>
                <td className="px-5 py-3.5 max-w-[240px]"><span className="text-[#9CA3AF] text-sm truncate block">{row.description}</span></td>
                <td className="px-5 py-3.5">
                  <span className="text-white text-sm font-mono tabular-nums">{row.stratCount}</span>
                </td>
                <td className="px-5 py-3.5">
                  <span className="text-[#9CA3AF] text-xs font-mono whitespace-nowrap">
                    {new Date(row.createdAt).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })}
                  </span>
                </td>
                <td className="px-5 py-3.5">
                  <div className="flex items-center gap-1.5">
                    <button
                      onClick={() => setModal({ open: true, mode: "edit", cat: row })}
                      className="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#141416] border border-transparent hover:border-[#2A2A2E] transition-all"
                    >
                      <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                        <path strokeLinecap="round" strokeLinejoin="round" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                      </svg>
                    </button>
                    <button
                      onClick={() => handleDelete(row.id)}
                      className="w-7 h-7 flex items-center justify-center rounded text-[#DC2626] hover:text-white hover:bg-[#DC2626] border border-transparent hover:border-[#DC2626] transition-all"
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
      </div>

      <CreateEditModal
        open={modal.open}
        mode={modal.mode}
        type="category"
        initial={modal.cat ? {
          id: modal.cat.id,
          name: modal.cat.name,
          side: modal.cat.side,
          description: modal.cat.description,
        } : undefined}
        onClose={() => setModal((p) => ({ ...p, open: false }))}
        onSave={handleSave as Parameters<typeof CreateEditModal>[0]["onSave"]}
      />
    </div>
  );
}
