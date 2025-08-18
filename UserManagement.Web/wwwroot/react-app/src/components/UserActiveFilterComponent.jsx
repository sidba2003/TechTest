export default function UserActiveFilterComponent(props) {
    return (
        <>
            <div className="filter-active-class filter-class">
                <label>
                    <input
                        type="checkbox"
                        checked={props.showActiveUsers}
                        onChange={() => props.displayActive(prev => !prev)}
                    />
                    Show Active Users
                </label>
            </div>
            <div className="filter-inactive-class filter-class">
                <label>
                    <input
                        type="checkbox"
                        checked={props.showInactiveUsers}
                        onChange={() => props.displayInactive(prev => !prev)}
                    />
                    Show Inactive Users
                </label>
            </div>
        </>
    )
}