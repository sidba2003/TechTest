import LogComponentDisplay from "./LogComponentDisplay.jsx";
import OperationFilters from "./OperationFilters.jsx";
import { useState, useMemo, useEffect } from "react";

export default function LogComponent(props) {
    const [showCreate, setShowCreate] = useState(true);
    const [showUpdate, setShowUpdate] = useState(true);
    const [showDelete, setShowDelete] = useState(true);

    // page & page size
    const initialSize =
        [5, 10, 20].includes(props.pageSize) ? props.pageSize : 10;
    const [pageSize, setPageSize] = useState(initialSize);
    const [page, setPage] = useState(1);

    // filter once, then paginate
    const filtered = useMemo(() => {
        return (props.operations || []).filter(
        (item) =>
            (showCreate && item.operation === "CREATE") ||
            (showUpdate && item.operation === "UPDATE") ||
            (showDelete && item.operation === "DELETE")
        );
    }, [props.operations, showCreate, showUpdate, showDelete]);

    const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));

    useEffect(() => {
        setPage(1);
    }, [showCreate, showUpdate, showDelete, props.operations, pageSize]);

    useEffect(() => {
        if (page > totalPages) setPage(totalPages);
    }, [page, totalPages]);

    const startIndex = (page - 1) * pageSize;
    const currentItems = filtered.slice(startIndex, startIndex + pageSize);

    const userOperationsList = currentItems.map((item, idx) => (
        <LogComponentDisplay
        key={item.id ?? item._id ?? item.timestamp ?? `${startIndex + idx}`}
        {...item}
        />
    ));

    return (
        <div className="user-operation-class">
            <OperationFilters
                showCreate={showCreate}
                setShowCreate={setShowCreate}
                showUpdate={showUpdate}
                setShowUpdate={setShowUpdate}
                showDelete={showDelete}
                setShowDelete={setShowDelete}
            />

            <div className="page-size">
                <label>
                Items per page:{" "}
                <select
                    value={pageSize}
                    onChange={(e) => setPageSize(parseInt(e.target.value, 10))}
                >
                    <option value={5}>5</option>
                    <option value={10}>10</option>
                    <option value={20}>20</option>
                </select>
                </label>
            </div>

            {userOperationsList}

            {totalPages > 1 && (
                <div className="pagination">
                <button onClick={() => setPage(1)} disabled={page === 1}>
                    First
                </button>
                <button
                    onClick={() => setPage((p) => Math.max(1, p - 1))}
                    disabled={page === 1}
                >
                    Prev
                </button>
                <span>
                    Page {page} of {totalPages}
                </span>
                <button
                    onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                    disabled={page === totalPages}
                >
                    Next
                </button>
                <button
                    onClick={() => setPage(totalPages)}
                    disabled={page === totalPages}
                >
                    Last
                </button>
                </div>
            )}
        </div>
    );
}
