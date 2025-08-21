export default function OperationFilters(props) {
    return (
        <div className="operations-filter-class">
            <div className="filter-class">
                <label>
                    <input
                        type="checkbox"
                        checked={props.showCreate}
                        onChange={() => props.setShowCreate(prev => !prev)}
                    />
                    Show CREATE
                </label>
            </div>
            <div className="filter-class">
                <label>
                    <input
                        type="checkbox"
                        checked={props.showUpdate}
                        onChange={() => props.setShowUpdate(prev => !prev)}
                    />
                    Show UPDATE
                </label>
            </div>
            <div className="filter-class">
                <label>
                    <input
                        type="checkbox"
                        checked={props.showDelete}
                        onChange={() => props.setShowDelete(prev => !prev)}
                    />
                    Show DELETE
                </label>
            </div>
        </div>
    );
}