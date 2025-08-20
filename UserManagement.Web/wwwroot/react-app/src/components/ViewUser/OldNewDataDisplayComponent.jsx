export default function OldNewDataDisplayComponent({oldData, newData}) {
    return (
        <>
            <span>Old Data</span>
            <span>{oldData}</span>
            <span>New Data</span>
            <span>{newData}</span>
        </>
    )
}