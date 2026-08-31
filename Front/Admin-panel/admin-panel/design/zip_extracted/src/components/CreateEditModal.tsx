import { useEffect, useState } from "react";
import type { Side } from "../data/mockData";
import { MAPS, CATEGORIES, ATTACK_OPERATORS, DEFENSE_OPERATORS } from "../data/mockData";

type Mode = "create" | "edit";
type EntityType = "strat" | "category";

interface FormData {
  name: string;
  side: Side;
  map: string;
  categories: string[];
  operators: string[];
  description: string;
  videoUrl: string;
}

interface Props {
  open: boolean;
  mode: Mode;
  type: EntityType;
  initial?: Partial<FormData> & { id?: string };
  onClose: () => void;
  onSave: (data: FormData) => void;
}

const defaultForm: FormData = {
  name: "",
  side: "Attack",
  map: MAPS[0],
  categories: [],
  operators: [],
  description: "",
  videoUrl: "",
};

function MultiSelect({ label, options, value, onChange }: { label: string; options: string[]; value: string[]; onChange: (v: string[]) => void }) {
  function toggle(opt: string) {
    onChange(value.includes(opt) ? value.filter((v) => v !== opt) : [...value, opt]);
  }
  return (
    <div>
      <label className="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">{label}</label>
      <div className="flex flex-wrap gap-1.5 p-3 bg-[#0B0B0C] border border-[#2A2A2E] rounded">
        {options.map((opt) => (
          <button
            key={opt}
            type="button"
            onClick={() => toggle(opt)}
            className={`px-2.5 py-1 rounded text-xs font-medium transition-all
              ${value.includes(opt)
                ? "bg-[#DC2626] text-white border border-[#DC2626]"
                : "bg-[#1C1C1F] text-[#9CA3AF] border border-[#2A2A2E] hover:border-[#DC2626] hover:text-white"
              }`}
          >
            {opt}
          </button>
        ))}
      </div>
    </div>
  );
}

export default function CreateEditModal({ open, mode, type, initial, onClose, onSave }: Props) {
  const [form, setForm] = useState<FormData>({ ...defaultForm, ...initial });

  useEffect(() => {
    if (open) setForm({ ...defaultForm, ...initial });
  }, [open]);

  useEffect(() => {
    const handler = (e: KeyboardEvent) => { if (e.key === "Escape") onClose(); };
    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [onClose]);

  if (!open) return null;

  const operatorOptions = form.side === "Attack" ? ATTACK_OPERATORS : DEFENSE_OPERATORS;

  function set<K extends keyof FormData>(key: K, value: FormData[K]) {
    setForm((prev) => ({ ...prev, [key]: value }));
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    onSave(form);
    onClose();
  }

  const title = `${mode === "create" ? "Create" : "Edit"} ${type === "strat" ? "Strat" : "Category"}`;

  return (
    <>
      <div className="fixed inset-0 z-[100] bg-black/75 backdrop-blur-[2px]" onClick={onClose} />
      <div className="fixed inset-0 z-[110] flex items-center justify-center p-6 pointer-events-none">
        <div
          className="bg-[#141416] border border-[#2A2A2E] rounded shadow-2xl w-full max-w-[600px] max-h-[85vh] flex flex-col animate-slide-up pointer-events-auto"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Header */}
          <div className="px-6 py-4 border-b border-[#2A2A2E] flex items-center justify-between shrink-0">
            <div>
              <h2 className="text-white font-bold text-base">{title}</h2>
              {initial?.id && <p className="text-[#9CA3AF] text-xs font-mono mt-0.5">{initial.id}</p>}
            </div>
            <button onClick={onClose} className="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] transition-colors">
              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit} className="flex-1 overflow-y-auto px-6 py-5 space-y-5">
            {/* Name */}
            <div>
              <label className="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">
                {type === "strat" ? "Strat Name" : "Category Name"} <span className="text-[#DC2626]">*</span>
              </label>
              <input
                required
                type="text"
                value={form.name}
                onChange={(e) => set("name", e.target.value)}
                placeholder={type === "strat" ? "e.g. Clubhouse Garage Double Breach" : "e.g. Vertical Play"}
                className="w-full bg-[#0B0B0C] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] px-3 py-2.5 focus:outline-none focus:border-[#DC2626] transition-colors"
              />
            </div>

            {/* Side */}
            <div>
              <label className="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Side <span className="text-[#DC2626]">*</span></label>
              <div className="flex gap-2">
                {(["Attack", "Defense"] as Side[]).map((s) => (
                  <button
                    key={s}
                    type="button"
                    onClick={() => { set("side", s); set("operators", []); }}
                    className={`flex-1 py-2 rounded text-sm font-semibold border transition-colors
                      ${form.side === s
                        ? s === "Attack"
                          ? "bg-[rgba(220,38,38,0.15)] border-[#DC2626] text-[#EF4444]"
                          : "bg-[rgba(37,99,235,0.15)] border-[#2563EB] text-[#60A5FA]"
                        : "bg-[#0B0B0C] border-[#2A2A2E] text-[#9CA3AF] hover:text-white"
                      }`}
                  >
                    {s === "Attack" ? "⚔" : "🛡"} {s}
                  </button>
                ))}
              </div>
            </div>

            {/* Map (strat only) */}
            {type === "strat" && (
              <div>
                <label className="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Map <span className="text-[#DC2626]">*</span></label>
                <select
                  value={form.map}
                  onChange={(e) => set("map", e.target.value)}
                  className="w-full bg-[#0B0B0C] border border-[#2A2A2E] rounded text-sm text-white px-3 py-2.5 focus:outline-none focus:border-[#DC2626] transition-colors"
                >
                  {MAPS.map((m) => <option key={m} value={m}>{m}</option>)}
                </select>
              </div>
            )}

            {/* Description */}
            <div>
              <label className="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Description</label>
              <textarea
                value={form.description}
                onChange={(e) => set("description", e.target.value)}
                rows={3}
                placeholder="Describe the strategy or category…"
                className="w-full bg-[#0B0B0C] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] px-3 py-2.5 focus:outline-none focus:border-[#DC2626] transition-colors resize-none"
              />
            </div>

            {/* Video URL (strat only) */}
            {type === "strat" && (
              <div>
                <label className="block text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Video URL</label>
                <input
                  type="url"
                  value={form.videoUrl}
                  onChange={(e) => set("videoUrl", e.target.value)}
                  placeholder="https://youtube.com/watch?v=…"
                  className="w-full bg-[#0B0B0C] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] px-3 py-2.5 focus:outline-none focus:border-[#DC2626] transition-colors"
                />
              </div>
            )}

            {/* Categories (strat only) */}
            {type === "strat" && (
              <MultiSelect
                label="Categories"
                options={CATEGORIES}
                value={form.categories}
                onChange={(v) => set("categories", v)}
              />
            )}

            {/* Operators (strat only) */}
            {type === "strat" && (
              <MultiSelect
                label={`Operators (${form.side})`}
                options={operatorOptions}
                value={form.operators}
                onChange={(v) => set("operators", v)}
              />
            )}
          </form>

          {/* Footer */}
          <div className="px-6 py-4 border-t border-[#2A2A2E] flex items-center justify-end gap-3 shrink-0">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 bg-[#1C1C1F] hover:bg-[#242428] text-[#9CA3AF] hover:text-white text-sm font-medium rounded border border-[#2A2A2E] transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              form=""
              onClick={handleSubmit as unknown as React.MouseEventHandler}
              className="px-5 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-sm font-semibold rounded transition-colors"
            >
              {mode === "create" ? "Create" : "Save Changes"}
            </button>
          </div>
        </div>
      </div>
    </>
  );
}
