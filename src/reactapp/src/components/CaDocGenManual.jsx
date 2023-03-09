import * as React from "react";
import {
    makeStyles,
    tokens,
    useId,
    Label,
    Textarea,
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
import { CaDocGenButton } from '../components/CaDocGenButton';

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
});


export const CaDocGenManual = () => {
    const textareaId = useId("textarea");
    const styles = useStyles();
    const [caPolicyJson, setCaPolicyJson] = useState("");

    return (
        <>
            <div className={styles.base}>
                <p>Use one of the options below to manually generate the documentation without signing into idPowerToys.</p>
                <p>Documentation generated manually will not include the names of users, apps and other directory objects.</p>

                <Accordion>
                    <AccordionItem value="1">
                        <AccordionHeader icon={<CalendarMonthRegular />}>
                            Graph Explorer
                        </AccordionHeader>
                        <AccordionPanel>
                            <ul>
                                <li>Open <a target="_blank" href="https://developer.microsoft.com/en-us/graph/graph-explorer?request=policies%2FconditionalAccessPolicies&method=GET&version=beta&GraphUrl=https://graph.microsoft.com">Graph Explorer</a></li>
                                <li>Run a query for <b>'https://graph.microsoft.com/beta/policies/conditionalAccessPolicies'</b></li>
                                <li>Highlight and copy the content from the <b>Response preview</b> panel</li>
                                <li>Paste the content below and click <b>Generate documentation</b></li>
                            </ul>
                        </AccordionPanel>
                    </AccordionItem>
                    <AccordionItem value="2">
                        <AccordionHeader icon={<VSCTerminalPowershell width={16} />}>
                            Graph PowerShell
                        </AccordionHeader>
                        <AccordionPanel>
                            <ul>
                                <li>Run this command to copy the policies to the clipboard and paste into the text box below.</li>
                                <li><small><code>Invoke-GraphRequest -Uri 'https://graph.microsoft.com/beta/policies/conditionalAccessPolicies' -OutputType Json | Set-Clipboard</code></small></li>
                            </ul>
                        </AccordionPanel>
                    </AccordionItem>
                </Accordion>
                <Label htmlFor={textareaId}>
                </Label>
                <Textarea
                    resize="both"
                    textarea={{ className: styles.textarea }}
                    id={textareaId}
                    value={caPolicyJson}
                    onChange={(e) => setCaPolicyJson(e.target.value)}
                />
            </div>
            <CaDocGenButton isManual={true} caPolicyJson={caPolicyJson} />
        </>
    );
};