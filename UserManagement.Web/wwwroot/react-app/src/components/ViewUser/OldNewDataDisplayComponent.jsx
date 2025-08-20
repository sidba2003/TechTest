export default function OldNewDataDisplayComponent({oldData, newData}) {
    return (
        <>
            <span>Old Data</span>
            <span>{oldData !== null ? oldData : "No data to show"}</span>
            <span>New Data</span>
            <span>{newData !== null ? newData : "No data to show"}</span>
        </>
    )
}