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
import { useState } from 'react';
import {
    Accordion,
    AccordionHeader,
    AccordionItem,
    AccordionPanel,
} from "@fluentui/react-components";
import { VSCTerminalPowershell } from '@icongo/vsc';
import { Providers, ProviderState } from "@microsoft/mgt";

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
        marginTop: tokens.spacingVerticalM,
        columnGap: "15px",
        display: "flex",
    },
});


export const CaDocGenAuto = () => {
    const textareaId = useId("textarea");
    const styles = useStyles();
    const [caPolicyJson, setCaPolicyJson] = useState("");

    const handleClick = async () => {

        let policy = {
            conditionalAccessPolicyJson: caPolicyJson,
            isManual: false
        };

        let token = "";
        if (Providers.globalProvider.state === ProviderState.SignedIn) {
            token = await Providers.globalProvider.getAccessToken({ scopes: ['User.Read'] })
        }

        const options = {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
                'X-PowerPointGeneration-Token': token
            },
            body: JSON.stringify(policy)
        };


        fetch('/weatherforecast', options)
            .then((response) => response.blob())
            .then((blob) => {

                // 2. Create blob link to download
                const url = window.URL.createObjectURL(new Blob([blob]));
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', `Conditional Access Policy Documentation.pptx`);
                // 3. Append to html page
                document.body.appendChild(link);
                // 4. Force download
                link.click();
                // 5. Clean up and remove the link
                link.parentNode.removeChild(link);

            })
            .catch((error) => {
                error.json().then((json) => {
                    this.setState({
                        errors: json,
                    });
                })
            });
    };

    return (
        <>
            <div className={styles.base}>
                <div className={styles.wrapper}>
                    <Button appearance="primary" icon={<CalendarMonthRegular />}
                        onClick={handleClick}
                    >
                        Generate documentation
                    </Button>
                </div>
            </div>
        </>
    );
};