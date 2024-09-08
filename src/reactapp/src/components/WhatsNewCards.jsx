import {
    makeStyles,
    shorthands,
    Button,
    Body1,
    tokens,
} from "@fluentui/react-components";
import {
    Card,
    CardHeader,
    CardFooter,
    CardPreview,
} from "@fluentui/react-components/unstable";
import { useNavigate } from "react-router-dom";

import {
    SlideLayout24Filled,
} from "@fluentui/react-icons";
import * as React from "react";

const flex = {
    ...shorthands.gap("16px"),
    display: "flex",
};

const useStyles = makeStyles({
    main: {
        display: "flex",
        flexWrap: "wrap",
        flexDirection: "column",
        columnGap: "16px",
        rowGap: "36px",
    },

    row: {
        ...flex,
        flexWrap: "wrap",
    },

    card: {
        width: "400px",
        maxWidth: "100%",
        height: "fit-content",
    },

    text: {
        ...shorthands.margin(0),
    },

    grayBackground: {
        backgroundColor: tokens.colorNeutralBackground3,
    },
});

const ConditionalAccessCard = (props) => {
    const styles = useStyles();
    const navigate = useNavigate();

    return (
        <div className={styles.main}>
            <div className={styles.row}>

                <Card className={styles.card} {...props}>
                    <CardPreview>
                        <img
                            src="./assets/whatsnew/cadocgen.png"
                            alt="Conditional Acccess Document Generated in PowerPoint."
                        />
                    </CardPreview>

                    <CardHeader
                        header={
                            <Body1>
                                <b>Conditional Access Documenter</b>
                            </Body1>
                        }
                    />

                    <p className={styles.text}>
                        Export your conditional access policies to PowerPoint for a bird's eye view of your security posture. Share you policies with security teams and stakeholders without granting them admin access to Microsoft Entra ID.</p>
                    <p className={styles.text}>
                        Your conditional access policies will never look the same again.
                    </p>

                    <CardFooter>
                        <Button appearance="primary" icon={<SlideLayout24Filled />}
                            onClick={() => navigate('/ca')}
                        >
                            Try it out
                        </Button>
                    </CardFooter>
                </Card>

                <Card className={styles.card} {...props}>
                    <CardPreview>
                        <img
                            src="./assets/whatsnew/mindmap.png"
                            alt="Microsoft Entra Mind map"
                        />
                    </CardPreview>

                    <CardHeader
                        header={
                            <Body1>
                                <b>Microsoft Entra Mind Map</b>
                            </Body1>
                        }
                    />

                    <p className={styles.text}>
                        This mindmap is a visual guide to Microsoft Entra and provides a high level overview of all the new products in the service. This new update includes maps for Microsoft Entra and Microsoft Entra ID.
                    </p>
                    <p className={styles.text}>
                        Download the latest version of the mind map and share it with your team.
                    </p>

                    <CardFooter>
                        <Button appearance="primary" icon={<SlideLayout24Filled />}
                            onClick={() => window.open('https://entra.news/p/entra-mind-maps', '_blank')}
                        >
                            View mind map
                        </Button>
                    </CardFooter>
                </Card>
            </div>
        </div>
    );
};

export const WhatsNewCards = () => {
    const styles = useStyles();

    return (
        <div className={styles.main}>
            <section>
                <ConditionalAccessCard />
            </section>
        </div>
    );
};
