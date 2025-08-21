export default function OldNewDataDisplayComponent({oldData, newData}) {
    return (
        <>
            <span><strong>Old User Data</strong></span>
            <span>{oldData !== null ? oldData : "No data to show"}</span>
            <span><strong>New User Data</strong></span>
            <span>{newData !== null ? newData : "No data to show"}</span>
        </>
    )
}