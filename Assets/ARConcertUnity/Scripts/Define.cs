public enum StageTargetType
{
    Normal,
    Large,
}

public static class Util
{
    // 추적 상태가 보임인지 여부 리턴
    public static bool IsTrackableStatusVisible(TrackableBehaviour.Status status)
    {
        return (status == TrackableBehaviour.Status.DETECTED
            || status == TrackableBehaviour.Status.TRACKED
            || status == TrackableBehaviour.Status.EXTENDED_TRACKED);
    }
}