import * as React from "react";
import {
    makeStyles,
    tokens,
    useId,
    Label,
    Textarea,
    Button
} from "@fluentui/react-components";
import {
    bundleIcon,
    CalendarMonthFilled,
    CalendarMonthRegular,
} from "@fluentui/react-icons";

const CalendarMonth = bundleIcon(CalendarMonthFilled, CalendarMonthRegular);

const useStyles = makeStyles({
    base: {
        display: "flex",
        flexDirection: "column",
        "& > label": {
            display: "block",
            marginBottom: tokens.spacingVerticalMNudge,
        },
    },
    textarea: {
        height: "200px",
    },
    wrapper: {
        columnGap: "15px",
        display: "flex",
    },
});

export const CaDocGenManual = () => {
    const textareaId = useId("textarea");
    const styles = useStyles();

    return (
        <>
            <div className={styles.base}>
                <Label htmlFor={textareaId}>Textarea with a height of 200px</Label>
                <Textarea
                    resize="both"
                    textarea={{ className: styles.textarea }}
                    id={textareaId}
                />
            </div>
            <div className={styles.wrapper}>
                <Button icon={<CalendarMonthRegular />}>Default</Button>
                <Button appearance="primary" icon={<CalendarMonthRegular />}>
                    Primary
                </Button>
            </div>
        </>
    );
};