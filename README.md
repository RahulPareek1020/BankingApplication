///MappingTable.tsx
import {
  Flexbox,
  Icons,
  Modal,
  Select,
  SingleDatePicker,
  SinglePickerValueType,
  Sizes,
  TBody,
  TextInput,
  TFoot,
  TH,
  THead,
  TR,
} from "@sede-x/shell-ds-react-framework";
import {
  ColumnDef,
  ColumnFiltersState,
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  Row,
  RowData,
  useReactTable,
} from "@tanstack/react-table";
import React, { useEffect, useMemo, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { FilterOption } from "../../context/tableContext/filter";
import {
  EditMappingRow,
  MappingData,
  MappingRow,
  MappingRuleData,
  MappingType,
  NominationType,
} from "../../context/tableContext/type";

import dayjs, { Dayjs } from "dayjs";

import {
  getMappingRuleDetails,
  getMappingTypes,
  getNominationDefinations,
  insertMappingRuleDetails,
  updateMappingRuleDetails,
} from "../../data/api";
import {
  StyledChildTableDiv,
  StyledClearButton,
  StyledDateDiv,
  StyledDisabledSelect,
  StyledDivChild,
  StyledDivFilter,
  StyledErrorLabel,
  StyledHeaderTR,
  StyledLabel,
  StyledNewMappingButton,
  StyledNoDataTd,
  StyledRawDiv,
  StyledRawTradeDiv,
  StyledSaveButton,
  StyledSelectDiv,
  StyledStar,
  StyledTable,
  StyledTd,
  StyledTdTopAlign,
  StyledTh,
  StyledTR,
  StyledTr,
} from "./MappingTable.style";
import { useAuth } from "react-oidc-context";
import {
  StyledModalText,
  StyledSpan,
} from "../NominationRun/NominationRun.style";
import { filterRoles } from "../../Utilities/RoleUtils";
import {
  sneOperator,
  sneReadOnly,
  Counterparty,
  ProdConFlag,
  ProdCon,
  FromMarketOperator,
  ToMarketOperator,
} from "../../constants/constants";

export const MappingTable = (args: any) => {
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [filterOption, setFilterOption] = useState<FilterOption>();
  const [loading, setLoading] = useState(true);
  const location = useLocation();
  const dataValue = location.state;
  const [data, setData] = useState([]);
  const [filterData, setfilterData] = useState([]);
  const [nomination, setnomination] = useState("");
  const [mappingType, setmappingType] = useState("");
  const [outputValue, setoutputValue] = useState("");
  const [applicableFrom, setapplicableFrom] = useState<Dayjs>();
  const [applicableTo, setapplicableTo] = useState<Dayjs>();
  const { user } = useAuth();
  const [hasUserValidRole, setHasUserValidRole] = useState(false);
  const [open, setOpen] = useState(false);
  const [showSecondaryInputBox, setShowSecondaryInputBox] = useState(false);
  const [initialDate, setinitialDate] =
    useState<SinglePickerValueType>(dayjs());
  const [initialEndDate, setInitialEndDate] = useState<SinglePickerValueType>(
    dayjs().set("year", 2050).set("month", 11).set("date", 31)
  );
  const [operation, setOperation] = useState("");
  const [isFormValid, setIsFormValid] = useState(false);
  const [errors, setErrors] = useState<FormErrors>({});
  const [responseMessage, setResponseMessage] = useState("");
  const [openConfirmation, setOpenConfirmation] = useState(false);
  const [resetText, setResetText] = useState("");
  const [nominationDefinitions, setnominationDefinitions] = useState<
    NominationType[]
  >([]);
  const [modifiedBy, setModifiedBy] = useState("");
  const [inputField1, setInputField1] = useState("");
  const [inputField2, setInputField2] = useState("");
  const [mappingRuleTypes, setMappingRuleTypes] = useState<MappingType[]>([]);
  const [mappingTypeId, setMappingTypeId] = useState(0);
  const [mappingId, setMappingId] = useState(0 || null);
  const [mappingInputId, setMappingInputId] = useState<number | null>(null);
  const [nominationDefinitionId, setNominationDefinitionId] = useState(0);
  const [dualMappingType, setDualMappingType] = useState(false);
  const [mappingRuleData, setMappingRuleData] = useState<MappingRuleData>();
  const [showConfirmModal, setShowConfirmModal] = useState(false);
  const [saveMappingRuleApiResponse, setSaveMappingRuleApiResponse] = useState("");
  const [actions, setActions] = useState<any>([]);
  const [confirmationMessage, setConfirmationMessage] = useState("");
  const [apiError, setApiError] = useState(false);
  const [isEdit, setIsEdit] = useState(false);
  const [reset, setReset] = useState("");
  const [copiedRowData, setCopiedRowData] = useState<EditMappingRow>();

  const [formData, setFormData] = useState<FormDataType>({
    mappingName: "",
    mappingRuleType: "",
    mappingRuleNomination: "",
    inputField1: "",
    inputValue1: "",
    inputField2: "",
    inputValue2: "",
    inputFrom: initialDate,
    inputTo: initialEndDate,
    outputValue: "",
    outputFrom: initialDate,
    outputTo: initialEndDate,
  });

  type FormErrors = Partial<Record<keyof FormData | string, string>>;

  type FormDataType = {
    mappingName: string;
    mappingRuleType: string;
    mappingRuleNomination: string;
    inputField1: string;
    inputValue1: string;
    inputField2: string;
    inputValue2: string;
    inputFrom: SinglePickerValueType;
    inputTo: SinglePickerValueType;
    outputValue: string;
    outputFrom: SinglePickerValueType;
    outputTo: SinglePickerValueType;
  };

  useEffect(() => {
    if (user?.access_token) {
      var roleInfo = filterRoles(user.access_token);
      if (roleInfo.role && roleInfo.role.length > 0) {
        if (!(roleInfo.role === sneReadOnly || roleInfo.role === sneOperator)) {
          setHasUserValidRole(true);
          setModifiedBy(roleInfo.name);
        }
      }
    }
    initializeFormData();
    setMappingTypeId(parseInt(mappingRuleTypes[0]?.value));
    setNominationDefinitionId(parseInt(nominationDefinitions[0]?.value));
    setData([]);
    setfilterData([]);
    setLoading(true);
    getInputMappingTypes();
    getMappingNominationDefinations();
    getMappingDetailsData(args?.aggPosRefId);
  }, []);

  useEffect(() => {
    if (data?.length != 0) {
      let filerData = data;
      let nominations = filerData.map((x: any) => x.nominationDefinitionName);
      nominations = nominations.filter(
        (value: any, index: any, self: any) => self.indexOf(value) === index
      );
      nominations = nominations.map((x: any) => ({
        value: x,
        label: x,
      }));
      let mappingTypes = filerData.map((x: any) => x.mappingTypeName);
      mappingTypes = mappingTypes.filter(
        (value: any, index: any, self: any) => self.indexOf(value) === index
      );

      mappingTypes = [
        { value: "Counterparty", label: "Counterparty" },
        { value: "Market", label: "Market" },
        { value: "DM Route", label: "DM Route" },
      ];
      let outputValues = filerData.map((x: any) => x.mappingOutputValue);
      outputValues = outputValues.filter(
        (value: any, index: any, self: any) => self.indexOf(value) === index
      );
      outputValues = outputValues.map((x: any) => ({
        value: x,
        label: x,
      }));
      let filterOptions: FilterOption = {
        nominations: nominations,
        mappingTypes: mappingTypes,
        outputValues: outputValues,
      };
      setFilterOption(filterOptions);
    }
  }, [data]);

  useEffect(() => {
    filterDataOnSelection();
  }, [nomination, mappingType, outputValue, applicableFrom, applicableTo]);
  const onSelectNomination = (nomination: string) => {
    setnomination(nomination);
  };

  useEffect(() => {
    if (!saveMappingRuleApiResponse) {
      setActions([
        {
          label: "Cancel",
          action: () => {
            handleConfirmationOnClose();
          },
        },
        {
          label: "Confirm",
          action: () => {
            handleConfirm();
          },
        },
      ]);
    } else {
      setActions([
        {
          label: "Ok",
          action: () => {
            handleOk();
          },
        },
      ]);
    }
  }, [confirmationMessage, saveMappingRuleApiResponse]);

  const onSelectMappingType = (mappingType: string) => {
    setmappingType(mappingType);
  };

  const onSelectOuptutValue = (outputValue: string) => {
    setoutputValue(outputValue);
  };

  const onSelectFromDate = (val: any) => {
    setapplicableFrom(val);
  };
  const onSelectToDate = (val: any) => {
    setapplicableTo(val);
  };

  const OnClearNomination = (nomination: string) => {
    setnomination("");
  };

  const OnClearMappingType = (nomination: string) => {
    setmappingType("");
  };

  const OnClearOutputValue = (nomination: string) => {
    setoutputValue("");
  };
  const OnClearApplicableFromDate = (nomination: string) => {
    setapplicableFrom(undefined);
  };
  const OnClearApplicableToDate = (nomination: string) => {
    setapplicableTo(undefined);
  };

  // Fetch Nomination Trade Details and format data
  const getMappingDetailsData = async (aggPosRefId: number) => {
    const result = await getMappingRuleDetails();
    result.sort((a: MappingData, b: MappingData) =>
      a.nominationDefinitionName.localeCompare(b.nominationDefinitionName)
    );
    result.sort((a: MappingData, b: MappingData) =>
      a.mappingTypeName.localeCompare(b.mappingTypeName)
    );
    result.sort((a: MappingData, b: MappingData) =>
      a.mappingMasterName.localeCompare(b.mappingMasterName)
    );

    const updatedData = result.map((item: any) => ({
      ...item,
      edit: "Edit",
    }));

    setfilterData(result);
    setData(result);
  };

  const closeModalClick = () => {
    args?.onClose(false);
  };

  const filterDataOnSelection = () => {
    let filterData = data;
    if (nomination != "") {
      filterData = filterData.filter(
        (x: any) => x.nominationDefinitionName == nomination
      );
    }

    if (mappingType != "") {
      filterData = filterData.filter(
        (x: any) => x.mappingTypeName == mappingType
      );
    }

    if (outputValue != "") {
      filterData = filterData.filter(
        (x: any) => x.mappingOutputValue == outputValue
      );
    }

    if (applicableFrom != undefined) {
      filterData = filterData.filter(
        (x: any) =>
          (applicableFrom.isAfter(dayjs(x.mappingOutputStartDate)) ||
            applicableFrom.isSame(dayjs(x.mappingOutputStartDate))) &&
          applicableFrom.isBefore(x.mappingOutputEndDate)
      );
    }

    if (applicableTo != undefined) {
      filterData = filterData.filter(
        (x: any) =>
          applicableTo.isAfter(dayjs(x.mappingOutputEndDate)) ||
          applicableTo.isSame(dayjs(x.mappingOutputEndDate))
      );
    }

    setfilterData(filterData);
  };

  const handleNewMapping = () => {
    setIsEdit(false);
    setOpen(true);
    initializeFormData();
    setMappingTypeId(parseInt(mappingRuleTypes[0]?.value));
    setNominationDefinitionId(parseInt(nominationDefinitions[0]?.value));
  };
  const handleOnClose = () => {
    setOpen(false);
  };

  const initializeFormData = () => {
    setOperation("New");
    setLoading(false);
    setShowSecondaryInputBox(false);
    resetFormData();
    setErrors({});
    setIsFormValid(false);
    setInputField1("");
    setInputField2("");
    setApiError(false);
    setConfirmationMessage("");
    setShowConfirmModal(false);
    setSaveMappingRuleApiResponse("");
    setDualMappingType(false);
    setMappingRuleData(undefined);
    setApiError(false);
    setMappingId(null);
    setMappingInputId(null);
    setReset("Clear");
  };

  const resetFormData = () => {
    setFormData({
      mappingName: "",
      mappingRuleType: "",
      mappingRuleNomination: "",
      inputField1: "",
      inputValue1: "",
      inputField2: "",
      inputValue2: "",
      inputFrom: initialDate,
      inputTo: initialEndDate,
      outputValue: "",
      outputFrom: initialDate,
      outputTo: initialEndDate,
    });
  };

  const handleClear = () => {
    setLoading(false);
    if (isEdit) {
      setFormData({
        mappingName:
          copiedRowData?.mappingName !== undefined
            ? copiedRowData?.mappingName
            : "",
        mappingRuleType:
          copiedRowData?.mappingTypeName !== undefined
            ? copiedRowData?.mappingTypeName
            : "",
        mappingRuleNomination:
          copiedRowData?.nomination !== undefined
            ? copiedRowData?.nomination
            : "",
        inputField1: inputField1,
        inputValue1:
          copiedRowData?.mappingInputValue1 !== undefined
            ? copiedRowData?.mappingInputValue1
            : "",
        inputField2: inputField1,
        inputValue2:
          copiedRowData?.mappingInputValue2 !== undefined
            ? copiedRowData?.mappingInputValue2
            : "",
        inputFrom:
          copiedRowData?.mappingInputStartDate !== undefined
            ? copiedRowData?.mappingInputStartDate
            : initialDate,
        inputTo:
          copiedRowData?.mappingInputEndDate !== undefined
            ? dayjs(copiedRowData?.mappingInputEndDate)
            : initialEndDate,
        outputValue:
          copiedRowData?.mappingOutputValue !== undefined
            ? copiedRowData?.mappingOutputValue
            : "",
        outputFrom:
          copiedRowData?.mappingOutputStartDate !== undefined
            ? dayjs(copiedRowData?.mappingOutputStartDate)
            : initialDate,
        outputTo:
          copiedRowData?.mappingOutputEndDate !== undefined
            ? dayjs(copiedRowData?.mappingOutputEndDate)
            : initialEndDate,
      });
    } else {
      initializeFormData();
      setMappingTypeId(parseInt(mappingRuleTypes[0]?.value));
      setNominationDefinitionId(parseInt(nominationDefinitions[0]?.value));
    }
  };

  const handleSave = () => {
    if (validate()) {
      setConfirmationMessage("Are you sure you want to save?");
      setSaveMappingRuleApiResponse("");
      setOpenConfirmation(true);
    }
  };

  const validate = (): boolean => {
    const formErrors: Partial<Record<keyof FormData | string, string>> = {};
    const isEmpty = (val: string | null | undefined) =>
      !val || val.trim() === "";

    // Validate Text fields
    if (isEmpty(formData.mappingName)) {
      formErrors.mappingName = "Mapping Name is required.";
    } else if (formData.mappingName.length > 255) {
      formErrors.mappingName = "Mapping Name must not exceed 255 characters.";
    }

    if (isEmpty(formData.inputValue1)) {
      formErrors.inputValue1 = "Input Value is required.";
    } else if (formData.inputValue1.length > 255) {
      formErrors.inputValue1 = "Input Value must not exceed 255 characters.";
    }

    if (showSecondaryInputBox && isEmpty(formData.inputValue2)) {
      formErrors.inputValue2 = "Input Value is required.";
    } else if (
      formData.inputValue2 !== undefined &&
      formData.inputValue2?.length > 255
    ) {
      formErrors.inputValue2 = "Input Value must not exceed 255 characters.";
    }

    if (isEmpty(formData.outputValue)) {
      formErrors.outputValue = "Output Value is required.";
    } else if (formData.outputValue.length > 255) {
      formErrors.outputValue = "Output Value must not exceed 255 characters.";
    }

    type DateFieldKeys = "inputFrom" | "inputTo" | "outputFrom" | "outputTo";

    const validateDates = (
      fromKey: DateFieldKeys,
      toKey: DateFieldKeys,
      errorKey: string,
      withinFromKey?: DateFieldKeys,
      withinToKey?: DateFieldKeys
    ) => {
      const from = dayjs(formData[fromKey]);
      const to = dayjs(formData[toKey]);

      if (!from || !from.isValid()) {
        formErrors[errorKey] = "From date is required.";
      } else if (!to || !to.isValid()) {
        formErrors[errorKey] = "To date is required.";
      } else if (from.isAfter(to)) {
        formErrors[errorKey] = "From date must be before To date.";
      } else if (withinFromKey && withinToKey) {
        const withinFrom = dayjs(formData[withinFromKey]);
        const withinTo = dayjs(formData[withinToKey]);

        if (withinFrom.isValid() && withinTo.isValid()) {
          const isOutsideRange =
            from.isBefore(withinFrom, "day") || to.isAfter(withinTo, "day");
          if (isOutsideRange) {
            formErrors[errorKey] =
              "Input date range must be within the output date range.";
          }
        }
      }
    };

    validateDates(
      "inputFrom",
      "inputTo",
      "inputDates",
      "outputFrom",
      "outputTo"
    );

    validateDates("outputFrom", "outputTo", "outputDates");
    setErrors(formErrors);

    return Object.keys(formErrors).length === 0;
  };

  const getMappingNominationDefinations = async () => {
    const result = await getNominationDefinations();

    let nominationDefinitions = result.map((x: any) => ({
      nominationName: x.nominationDefinitionName,
      nominationDefinitionId: x.nominationDefinitionId,
    }));
    nominationDefinitions = nominationDefinitions.filter(
      (value: any, index: any, self: any) => self.indexOf(value) === index
    );

    nominationDefinitions = nominationDefinitions.filter(
      (value: any, index: any, self: any) =>
        index ===
        self.findIndex(
          (nomDef: { nominationName: any; nominationDefinitionId: any }) =>
            nomDef.nominationName === value.nominationName &&
            nomDef.nominationDefinitionId === value.nominationDefinitionId
        )
    );

    let updatedNominationDefinitions = nominationDefinitions.map(
      (item: { nominationName: any; nominationDefinitionId: any }) => ({
        label: item.nominationName,
        value: item.nominationDefinitionId,
      })
    );
    setNominationDefinitionId(updatedNominationDefinitions[0]?.value);
    setnominationDefinitions(updatedNominationDefinitions);
  };

  const getInputMappingTypes = async () => {
    const result = await getMappingTypes();

    let mappingTypes = result.map((x: any) => ({
      MappingTypeId: x.mappingTypeId,
      MappingTypeName: x.mappingTypeName,
    }));

    let updatedMappingTypes = mappingTypes.map(
      (item: { MappingTypeId: any; MappingTypeName: any }) => ({
        label: item.MappingTypeName,
        value: item.MappingTypeId,
      })
    );
    setInputField1(updatedMappingTypes[0]?.label);
    setMappingTypeId(updatedMappingTypes[0]?.value);
    setMappingRuleTypes(updatedMappingTypes);
  };

  const handleSelectChange =
    (field: keyof FormDataType) => (value: string, option: any) => {
      if (field === "mappingRuleType") {
        const mappingTypeId = parseInt(value);
        setMappingTypeId(mappingTypeId);
        initializeFormData();
        if (mappingTypeId === 4) {
          setShowSecondaryInputBox(true);
          setInputField1(Counterparty);
          setInputField2(ProdConFlag);
          setDualMappingType(true);
        } else {
          setShowSecondaryInputBox(false);
          setInputField1(option?.label);
        }
      }

      if (field === "mappingRuleNomination") {
        setNominationDefinitionId(parseInt(value));
      }

      setFormData((prev) => ({
        ...prev,
        [field]: option?.label,
      }));
    };

  const handleDateChange = (name: string, date: Dayjs | null) => {
    setFormData((prev) => ({
      ...prev,
      [name]: date ? date.toDate() : null,
    }));

    setErrors((prev) => {
      const formErrors = { ...prev };
      delete formErrors[name];
      return formErrors;
    });
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));

    setErrors((prev) => {
      const formErrors = { ...prev };
      delete formErrors[name];
      return formErrors;
    });
  };

  const columns = useMemo<ColumnDef<unknown, any>[]>(
    () => [
      {
        header: "Mapping Details",
        columns: [
          {
            id: "0",
            header: "Mapping Name",
            accessorKey: "mappingMasterName",
            meta: { enableRowSpan: true },
          },
          {
            id: "1",
            header: "Mapping Type",
            accessorKey: "mappingTypeName",
            meta: { enableRowSpan: true },
          },
          {
            id: "2",
            accessorKey: "nominationDefinitionName",
            header: "Nomination",
           meta: { enableRowSpan: true },
          },
        ],
      },
      {
        header: "Mapping Input",
        columns: [
          {
            id: "3",
            header: "Input Field",
            accessorKey: "mappingInputField",
          },
          {
            id: "4",
            header: "Input Value",
            accessorKey: "mappingInputValue",
          },
          {
            id: "5",
            header: "Valid From",
            accessorKey: "mappingInputStartDate",
            cell: (info) => {
              return dayjs(info.getValue()).format("D/M/YYYY");
            },
          },
          {
            id: "6",
            header: "Valid To",
            accessorKey: "mappingInputEndDate",
            cell: (info) => {
              return dayjs(info.getValue()).format("D/M/YYYY");
            },
          },
        ],
      },
      {
        header: "Mapping Output",
        columns: [
          {
            id: "7",
            header: "Output Value",
            accessorKey: "mappingOutputValue",
          },
          {
            id: "8",
            header: "Valid From",
            accessorKey: "mappingOutputStartDate",
            cell: (info) => {
              return dayjs(info.getValue()).format("D/M/YYYY");
            },
          },
          {
            id: "9",
            header: "Valid To",
            accessorKey: "mappingOutputEndDate",
            cell: (info) => {
              return dayjs(info.getValue()).format("D/M/YYYY");
            },
          },
          {
            id: "10",
            header: "Last Modified By",
            accessorKey: "mappingMasterModifiedBy",
          },
          {
            id: "11",
            header: "Last Modified Date Time",
            accessorKey: "mappingMasterModifiedDate",
            cell: (info) => {
              return dayjs(info.getValue()).format("D/M/YYYY HH:mm");
            },
          },
        ],
      },
      {
        id: "12",
        header: "Edit",
        accessorKey: "edit",
        cell: (info) => {
          const row = info.row.original;

          function handleRowClick(row: any): void {
            setIsEdit(true);
            setMappingId(row.mappingMasterId);
            setMappingInputId(row.mappingInputId);
            setMappingTypeId(row.mappingTypeId);
            setErrors({});
            setReset("Reset");
            let inputField1 = "";
            let inputValue1 = "";
            let inputField2 = "";
            let inputValue2 = "";
            if (row.mappingTypeId === 2 || row.mappingTypeId === 4) {
              if (row.mappingTypeId === 2) {
                setInputField1("Market");
                setInputField2("");
                setShowSecondaryInputBox(false);
                inputField1 = row?.mappingInputField;
                inputValue1 = row?.mappingInputValue;
              } else if (row.mappingTypeId === 4) {
                const table = info.table;
                const allRows = table.getRowModel().rows;
                const prevRow =
                  info.row.index > 0
                    ? (allRows[info.row.index - 1] as Row<MappingRow>)
                    : null;
                const prevRowData = prevRow?.original as MappingRow;
                const nextRow =
                  info.row.index >= 0
                    ? (allRows[info.row.index + 1] as Row<MappingRow>)
                    : null;
                const nextRowData = nextRow?.original as MappingRow;

                let considerPrevRow = false;
                let considerNextRow = false;

                if (prevRowData && nextRowData) {
                  if( row.mappingMasterId === prevRowData.mappingMasterId
                    && row.mappingOutputValue === prevRowData.mappingOutputValue) {
                      considerPrevRow = true;
                    }
                   else if( row.mappingMasterId === nextRowData.mappingMasterId 
                    && row.mappingOutputValue === nextRowData.mappingOutputValue) {
                      considerNextRow = true;
                    }
                } else if (nextRowData) {
                    considerNextRow =
                        row.mappingMasterId === nextRowData.mappingMasterId;
                  } else if (prevRowData) {
                  considerPrevRow =
                    row.mappingMasterId === prevRowData.mappingMasterId;
                }

                setInputField1(Counterparty);
                setInputField2(ProdConFlag);
                if (considerPrevRow) {
                  inputField1 = prevRowData?.mappingInputField;
                  inputValue1 = prevRowData?.mappingInputValue;
                  inputField2 = row?.mappingInputField;
                  inputValue2 = row?.mappingInputValue;
                } else {
                  inputField1 = row?.mappingInputField;
                  inputValue1 = row?.mappingInputValue;
                  inputField2 = nextRowData?.mappingInputField;
                  inputValue2 = nextRowData?.mappingInputValue;
                }

                setShowSecondaryInputBox(true);
              } else {
                inputField1 = row?.mappingInputField;
                inputValue1 = row?.mappingInputValue;
                setShowSecondaryInputBox(false);
              }

              setFormData({
                mappingName: row.mappingMasterName,
                mappingRuleType: row.mappingTypeName,
                mappingRuleNomination: row.nominationDefinitionName,
                inputField1: inputField1,
                inputValue1: inputValue1,
                inputField2: inputField2,
                inputValue2: inputValue2,
                inputFrom: row.mappingInputStartDate,
                inputTo: row.mappingInputEndDate,
                outputValue: row.mappingOutputValue,
                outputFrom: row.mappingOutputStartDate,
                outputTo: row.mappingOutputEndDate,
              });
            } else {
              setShowSecondaryInputBox(false);
              inputField1 = row.mappingInputField;
              inputValue1 = row.mappingInputValue;
              inputField2 = "";
              inputValue2 = "";
              setFormData({
                mappingName: row.mappingMasterName,
                mappingRuleType: row.mappingTypeName,
                mappingRuleNomination: row.nominationDefinitionName,
                inputField1: inputField1,
                inputValue1: inputValue1,
                inputFrom: row.mappingInputStartDate,
                inputTo: row.mappingInputEndDate,
                inputField2: inputField2,
                inputValue2: inputValue2,
                outputValue: row.mappingOutputValue,
                outputFrom: row.mappingOutputStartDate,
                outputTo: row.mappingOutputEndDate,
              });
            }
            setLoading(false);
            setOpen(true);
            setOperation("Edit");
            setCopiedRowData({
              mappingInputField1: inputField1,
              mappingInputValue1: inputValue1,
              mappingInputField2: inputField2,
              mappingInputValue2: inputValue2,
              mappingInputStartDate: row.mappingInputStartDate,
              mappingInputEndDate: row.mappingInputEndDate,
              mappingName: row.mappingMasterName,
              mappingOutputValue: row.mappingOutputValue,
              mappingOutputStartDate: row.mappingOutputStartDate,
              mappingOutputEndDate: row.mappingOutputEndDate,
              mappingTypeName: row.mappingTypeName,
              mappingInputId: row.mappingInputId,
              mappingMasterId: row.mappingMasterId,
              nomination: row.nominationDefinitionName,
              edit: "edit",
            });
          }

          return (
            <a href={`#`} onClick={() => handleRowClick(row)} style={{color:'inherit'}}>
              Edit
            </a>
          );
        },
      },
    ],
    []
  );

  // Render rows function to group rows based on data
    const renderRows = (rows: Row<any>[]) => {
    rows.map((row: any, i: number, rows) => {
      const topRow: any = rows[i - 1];
      for (let j = 0; j < row.getVisibleCells().length; j++) {
        let cell = row.getVisibleCells()[j];
        let cell2 = row.getVisibleCells()[1];
        let cell3 = row.getVisibleCells()[2];
        if (
          !cell.column.columnDef.meta?.enableRowSpan ||
          !topRow ||
          topRow?.getIsGrouped() ||
          row?.getIsGrouped()
        ) {
          cell.rowSpan = 1;
          cell.isRowSpanned = false;
          continue;
        }

        // Grouping logic for cells
        const getMergeTopCell = (ri: number, ci: number): any => {
          debugger;
          const topRow: any = rows[ri - 1];
          const cell: any = (rows[ri] as any).getVisibleCells()[ci];
          if (topRow != undefined) {
            const topCell: any = topRow?.getVisibleCells()[ci];
            const topCelll0: any = topRow?.getVisibleCells()[0];
            const topCelll1: any = topRow?.getVisibleCells()[1];
            const topCelll2: any = topRow?.getVisibleCells()[2];
            const topCelll4: any = topRow?.getVisibleCells()[7];

            const celll0: any = (rows[ri] as any).getVisibleCells()[0];
            const celll1: any = (rows[ri] as any).getVisibleCells()[1];
            const celll2: any = (rows[ri] as any).getVisibleCells()[2];
            const celll4: any = (rows[ri] as any).getVisibleCells()[7];
            //alert(topCelll4.getValue())
            if (
              !(
                JSON.stringify(topCelll0.getValue()) ===
                  JSON.stringify(celll0.getValue()) &&
                JSON.stringify(topCelll1.getValue()) ===
                  JSON.stringify(celll1.getValue()) &&
                JSON.stringify(topCelll2.getValue()) ===
                  JSON.stringify(celll2.getValue())
                  &&
                JSON.stringify(topCelll4.getValue()) ===
                  JSON.stringify(celll4.getValue())
              )
            )
              return cell;

            if (
              !topRow ||
              topRow.getIsGrouped() ||
              JSON.stringify(topCell.getValue()) !==
                JSON.stringify(cell.getValue())
            ) {
              return cell;
            } else {
              return getMergeTopCell(ri - 1, ci);
            }
          } else {
            return cell;
          }
        };

        let topCell = topRow.getVisibleCells()[j];
        let topCell1 = topRow.getVisibleCells()[1];
        let topCell2 = topRow.getVisibleCells()[2];

        if (
          JSON.stringify(topCell.getValue()) ===
            JSON.stringify(cell.getValue()) &&
          JSON.stringify(topCell1.getValue()) ===
            JSON.stringify(cell2.getValue()) &&
          JSON.stringify(topCell2.getValue()) ===
            JSON.stringify(cell3.getValue())
        ) {
          getMergeTopCell(i, j).rowSpan += 1;
          cell.isRowSpanned = true;
        } else {
          cell.rowSpan = 1;
          cell.isRowSpanned = false;
        }
      }

      return null;
    });

    return rows.map((row: any) => {
      return (
        <StyledTr key={row.id}>
          {row.getVisibleCells().map((cell: any) => {
            if (cell.isRowSpanned) return null;
            else {
              return (
                <StyledTd
                  key={cell.id}
                  rowSpan={cell.rowSpan}
                  style={cell.column.id == "3" ? { textAlign: "left" } : {}}
                >
                  {flexRender(cell.column.columnDef.cell, cell.getContext())}
                </StyledTd>
              );
            }
          })}
        </StyledTr>
      );
    });
  };

  const [columnOrder, setColumnOrder] = useState<String[]>(
    columns.map((column) => column.id as string)
  );
  const tableOptions = {
    state: {
      columnOrder,
    },
    onColumnOrderChange: setColumnOrder,
  };

  const table = useReactTable({
    data: filterData,
    columns,
    filterFns: {},
    state: {
      columnFilters,
    },
    onColumnFiltersChange: setColumnFilters,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
  });

  const handleConfirmationOnClose = () => {
    setOpenConfirmation(false);
  };

  const handleConfirm = async () => {
    setConfirmationMessage("");
    let mappingInputField1 = "";
    let mappingInputValue1 = "";
    let mappingInputValue2 = "";
    let mappingInputField2 = "";

    if (mappingTypeId === 4) {
      //DM Route
      mappingInputField1 = Counterparty;
      mappingInputValue1 = formData.inputValue1.trim();
      mappingInputField2 = ProdCon;
      mappingInputValue2 = formData.inputValue2.trim();
    } else if (mappingTypeId === 2) {
      //Market
      mappingInputField1 = FromMarketOperator;
      mappingInputValue1 = formData.inputValue1.trim();
      mappingInputField2 = ToMarketOperator;
      mappingInputValue2 = formData.inputValue1.trim();
    } else {
      mappingInputField1 =
        inputField1.trim() === "" ? mappingRuleTypes[0]?.label : inputField1;
      mappingInputValue1 = formData.inputValue1.trim();
      mappingInputField2 = "";
      mappingInputValue2 = "";
    }

    const model: MappingRuleData = {
      NominationDefinitionId: nominationDefinitionId,
      MappingName: formData.mappingName.trim(),
      MappingId: mappingId,
      MappingInputId: mappingInputId,
      MappingTypeId:
        mappingTypeId === 0
          ? parseInt(mappingRuleTypes[0]?.value)
          : mappingTypeId,
      MappingInputField1:
        mappingTypeId === 4
          ? mappingInputField1.toLocaleUpperCase()
          : mappingInputField1,
      MappingInputValue1: mappingInputValue1,
      MappingInputField2:
        mappingTypeId === 4
          ? mappingInputField2.toLocaleUpperCase()
          : mappingInputField2,
      MappingInputValue2: mappingInputValue2,
      MappingInputValidFrom: formData.inputFrom ?? dayjs(),
      MappingInputValidTo: formData.inputTo ?? dayjs(),
      MappingOutputValidFrom: formData.outputFrom ?? dayjs(),
      MappingOutputValidTo: formData.outputTo ?? dayjs(),
      MappingOutputValue: formData.outputValue.trim(),
      ModifiedBy: modifiedBy,
      MappingResult: 0,
    };

    setMappingRuleData(model);

    try {
      const response = !isEdit
        ? await insertMappingRuleDetails(model)
        : await updateMappingRuleDetails(model);
      let message = "";

      if (response && response.message) {
        if (response.message.startsWith("Success")) {
          message = response.message.replace("Success: ", "").trim();
          setApiError(false);
          getMappingDetailsData(args?.aggPosRefId);
        } else if (
          response.message.startsWith("Error") ||
          response.message.startsWith("Warning")
        ) {
          message = response.message.replace("Error: ", "").trim();
          setApiError(true);
        } else if (response.message.startsWith("Fatal")) {
          message = response.message.replace("Fatal: ", "").trim();
          setApiError(false);
        }

        setSaveMappingRuleApiResponse(message);
      } else {
        setApiError(true);
        message = "Unexpected response from server.";
      }
    } catch (error) {
      setLoading(false);
      console.error("Error saving mapping rule:", error);
    }
  };

  const handleConfirmNo = () => {
    setOpenConfirmation(false);
  };

  const handleOk = () => {
    handleConfirmNo();
    setResponseMessage("");
    setOpenConfirmation(false);
    if (apiError) {
      setOpen(true);
    } else {
      setOpen(false);
    }
  };

  return (
    <>
      <Modal
        open={open}
        loading={loading}
        showHeader={false}
        bodyStyle={{ flex: 0 }}
        size={Sizes.Medium}
        onClose={() => {
          handleOnClose();
        }}
        maskClosable = {false}
      >
        <h2>{operation} Mapping</h2>
        <StyledTable>
          <StyledTR>
            <StyledTdTopAlign>
              <h3>Mapping Details</h3>
            </StyledTdTopAlign>
            <StyledTd>
              <Flexbox>
                <StyledLabel
                  htmlFor="MappingName"
                  style={{ margin: "0px", width: "6vw", textAlign: "left" }}
                >
                  Mapping Name<StyledStar>*</StyledStar>
                </StyledLabel>
                <TextInput
                  title="Mapping Name"
                  type="text"
                  id="txtMappingName"
                  name="mappingName"
                  onChange={handleChange}
                  value={formData.mappingName}
                  style={{ color: "inherit" }}
                  disabled={isEdit}
                ></TextInput>
              </Flexbox>
              {errors.mappingName && (
                <Flexbox>
                  <StyledErrorLabel htmlFor="MappingName">
                    {errors.mappingName}
                  </StyledErrorLabel>
                </Flexbox>
              )}
              <StyledSpan></StyledSpan>
              <Flexbox>
                <StyledLabel
                  htmlFor="MappingType"
                  style={{ margin: "0px", width: "7vw", textAlign: "left" }}
                >
                  Mapping Type<StyledStar>* </StyledStar>
                </StyledLabel>
                {!isEdit && (
                  <Select
                    id="mappingRuleType"
                    name="mappingRuleType"
                    allowClear={false}
                    options={mappingRuleTypes}
                    optionFilterProp="label"
                    onChange={handleSelectChange("mappingRuleType")}
                    value={
                      formData.mappingRuleType === ""
                        ? mappingRuleTypes[0]?.label
                        : formData.mappingRuleType
                    }
                    style={{ textAlign: "left", color: "inherit" }}
                    {...args}
                  />
                )}
                {isEdit && (
                  <TextInput
                    title="Mapping Name"
                    type="text"
                    id="txtmappingRuleType"
                    name="txtmappingRuleType"
                    value={formData.mappingRuleType}
                    style={{ color: "inherit" }}
                    disabled={isEdit}
                  ></TextInput>
                )}
              </Flexbox>
              {errors.mappingRuleType && (
                <Flexbox>
                  <StyledErrorLabel htmlFor="MappingRuleType">
                    {errors.mappingRuleType}
                  </StyledErrorLabel>
                </Flexbox>
              )}
              <StyledSpan></StyledSpan>
              <Flexbox>
                <StyledLabel
                  htmlFor="Nomination"
                  style={{ margin: "0px", width: "7vw", textAlign: "left" }}
                >
                  Nomination<StyledStar>* </StyledStar>
                </StyledLabel>
               {!isEdit && <Select
                  id="mappingRuleNomination"
                  name="mappingRuleNomination"
                  allowClear={false}
                  options={nominationDefinitions}
                  optionFilterProp="label"
                  onChange={handleSelectChange("mappingRuleNomination")}
                  value={
                    formData.mappingRuleNomination === ""
                      ? nominationDefinitions[0]?.label
                      : formData.mappingRuleNomination
                  }
                  style={{ textAlign: "left", color: "inherit" }}
                  {...args}
                  disabled={isEdit}
                />
              }
                  {isEdit && <TextInput
                    title="Mapping Rule"
                    type="text"
                    id="txtmmappingRuleNomination"
                    name="txtmmappingRuleNomination"
                    value={formData.mappingRuleNomination}
                    style={{ color: "inherit" }}
                    disabled={isEdit}
                  ></TextInput>
              }
              </Flexbox>
              {errors.mappingRuleNomination && (
                <Flexbox>
                  <StyledErrorLabel htmlFor="Nomination">
                    {errors.mappingRuleNomination}
                  </StyledErrorLabel>
                </Flexbox>
              )}
            </StyledTd>
          </StyledTR>
          <StyledTR>
            <StyledTdTopAlign>
              <h3>Mapping Input</h3>
            </StyledTdTopAlign>
            <StyledTd>
              <StyledSpan></StyledSpan>
              <Flexbox>
                <StyledLabel
                  htmlFor="inputValue1"
                  style={{ margin: "0px", width: "7vw", textAlign: "left" }}
                >
                  {inputField1 === ""
                    ? mappingRuleTypes[0]?.label
                    : inputField1}
                  <StyledStar>*</StyledStar>
                </StyledLabel>
                <TextInput
                  title="Input Value"
                  type="text"
                  id="txtInputValue1"
                  name="inputValue1"
                  value={formData.inputValue1}
                  onChange={handleChange}
                  style={{ color : "inherit" }}
                ></TextInput>
              </Flexbox>
              {errors.inputValue1 && (
                <Flexbox>
                  <StyledErrorLabel htmlFor="InputValue1">
                    {errors.inputValue1}
                  </StyledErrorLabel>
                </Flexbox>
              )}
              <StyledSpan></StyledSpan>

              {showSecondaryInputBox && (
                <StyledSpan>
                  <StyledSpan></StyledSpan>
                  <Flexbox>
                    <StyledLabel
                      htmlFor="InputValue2"
                      style={{ margin: "0px", width: "7vw", textAlign: "left" }}
                    >
                      {inputField2}
                      <StyledStar>*</StyledStar>
                    </StyledLabel>
                    <TextInput
                      title="Input Value"
                      type="text"
                      id="txtInputValue2"
                      name="inputValue2"
                      onChange={handleChange}
                      value={formData.inputValue2}
                      style={{ color : "inherit" }}
                    ></TextInput>
                  </Flexbox>
                  {errors.inputValue2 && (
                    <Flexbox>
                      <StyledErrorLabel htmlFor="inputValue2">
                        {errors.inputValue2}
                      </StyledErrorLabel>
                    </Flexbox>
                  )}
                  <StyledSpan></StyledSpan>
                </StyledSpan>
              )}
              <Flexbox>
                <StyledLabel
                  style={{ margin: "0px", width: "7vw", textAlign: "left" }}
                >
                  Valid From<StyledStar>*</StyledStar>
                </StyledLabel>
                <SingleDatePicker
                  name="inputFrom"
                  allowClear={false}
                  placeholder={"Valid From"}
                  onChange={(date) => handleDateChange("inputFrom", date)}
                  value={formData.inputFrom ? dayjs(formData.inputFrom) : null}
                />
              </Flexbox>
              <StyledSpan></StyledSpan>
              <Flexbox>
                <StyledLabel
                  style={{ margin: "0px", width: "7vw", textAlign: "left" }}
                >
                  Valid To<StyledStar>*</StyledStar>
                </StyledLabel>
                <SingleDatePicker
                  name="inputTo"
                  allowClear={false}
                  placeholder={"Valid To"}
                  onChange={(date) => handleDateChange("inputTo", date)}
                  value={formData.inputTo ? dayjs(formData.inputTo) : null}
                />
              </Flexbox>
              {errors.inputDates && (
                <Flexbox>
                  <StyledErrorLabel htmlFor="inputDates">
                    {errors.inputDates}
                  </StyledErrorLabel>
                </Flexbox>
              )}
            </StyledTd>
          </StyledTR>
          <StyledTR>
            <StyledTdTopAlign>
              <h3>Mapping Output</h3>
            </StyledTdTopAlign>
            <StyledTd>
              <Flexbox>
                <StyledLabel
                  htmlFor="editNomination"
                  style={{ width: "7vw", textAlign: "left" }}
                >
                  Output Value<StyledStar>*</StyledStar>
                </StyledLabel>
                <TextInput
                  title="Output Value"
                  type="text"
                  id="txtOutputValue"
                  name="outputValue"
                  onChange={handleChange}
                  value={formData.outputValue}
                  style={{ margin: "0px", textAlign: "left" }}
                ></TextInput>
              </Flexbox>
              {errors.outputValue && (
                <Flexbox>
                  <StyledErrorLabel htmlFor="OutputValue">
                    {errors.outputValue}
                  </StyledErrorLabel>
                </Flexbox>
              )}
              <StyledSpan></StyledSpan>
              <Flexbox>
                <StyledLabel style={{ width: "7vw", textAlign: "left" }}>
                  Valid From<StyledStar>*</StyledStar>
                </StyledLabel>
                <SingleDatePicker
                  name="outputFrom"
                  allowClear={false}
                  placeholder={"Valid From"}
                  onChange={(date) => handleDateChange("outputFrom", date)}
                  value={
                    formData.outputFrom ? dayjs(formData.outputFrom) : null
                  }
                />
              </Flexbox>

              <StyledSpan></StyledSpan>
              <Flexbox>
                <StyledLabel
                  style={{ margin: "0px", width: "7vw", textAlign: "left" }}
                >
                  Valid To<StyledStar>*</StyledStar>
                </StyledLabel>
                <SingleDatePicker
                  name="outputTo"
                  allowClear={false}
                  placeholder={"Valid To"}
                  onChange={(date) => handleDateChange("outputTo", date)}
                  value={formData.outputTo ? dayjs(formData.outputTo) : null}
                />
              </Flexbox>
              {errors.outputDates && (
                <Flexbox>
                  <StyledErrorLabel htmlFor="outputFrom">
                    {errors.outputDates}
                  </StyledErrorLabel>
                </Flexbox>
              )}
            </StyledTd>
          </StyledTR>
          <StyledTR>
            <StyledTd>
              <StyledClearButton onClick={handleClear}>
                {reset}
              </StyledClearButton>
            </StyledTd>
            <StyledTd>
              <StyledSaveButton onClick={handleSave}>Save</StyledSaveButton>
            </StyledTd>
          </StyledTR>
        </StyledTable>
      </Modal>
      <Modal
        open={openConfirmation}
        loading={loading}
        showHeader={false}
        bodyStyle={{ flex: 0 }}
        size={Sizes.Small}
        onClose={() => {
          handleConfirmationOnClose();
        }}
        actions={actions}
      >
        <StyledModalText>
          {saveMappingRuleApiResponse === "" && confirmationMessage}
          {saveMappingRuleApiResponse !== "" && saveMappingRuleApiResponse}
        </StyledModalText>
      </Modal>
      <StyledDivFilter>
        <Flexbox style={{ margin: "5px" }}>
          <StyledSelectDiv>
            <StyledLabel htmlFor="nomination">Nomination</StyledLabel>
            <Select
              id="nomination"
              options={filterOption?.nominations}
              optionFilterProp="label"
              onSelect={onSelectNomination}
              onClear={OnClearNomination}
              {...args}
            />
          </StyledSelectDiv>
          <StyledSelectDiv>
            <StyledLabel htmlFor="mappingType" style={{ margin: "9px" }}>
              Mapping Type
            </StyledLabel>
            <Select
              id="mappingType"
              options={filterOption?.mappingTypes}
              optionFilterProp="label"
              onSelect={onSelectMappingType}
              onClear={OnClearMappingType}
              {...args}
            />
          </StyledSelectDiv>
          <StyledSelectDiv>
            <StyledLabel htmlFor="outputValue">Output Value</StyledLabel>
            <Select
              id="outputValue"
              options={filterOption?.outputValues}
              optionFilterProp="label"
              onSelect={onSelectOuptutValue}
              onClear={OnClearOutputValue}
              {...args}
            />
          </StyledSelectDiv>
        </Flexbox>
        <Flexbox>
          <StyledDateDiv>
            <StyledLabel style={{ margin: "0px" }}>Applicable From</StyledLabel>
            <SingleDatePicker
              placeholder={"From"}
              onChange={onSelectFromDate}
            />
          </StyledDateDiv>
          <StyledDateDiv>
            <StyledLabel style={{ margin: "10px" }}>Applicable To</StyledLabel>
            <SingleDatePicker placeholder={"To"} onChange={onSelectToDate} />
          </StyledDateDiv>
        </Flexbox>
      </StyledDivFilter>
      <StyledDivChild>
        <StyledRawDiv>
          <StyledRawTradeDiv>
            <StyledTable style={{ width: "100%", border: 0 }}>
              <StyledTr>
                <StyledTd style={{ width: "70%", border: 0 }}>
                  <h2
                    style={{
                      fontSize: 16,
                      padding: "0px 8px",
                      textAlign: "left",
                      lineHeight: "1px",
                      fontWeight: 600,
                    }}
                  >
                    Latest Mapping Details
                  </h2>
                </StyledTd>
                {hasUserValidRole && (
                  <StyledTd style={{ width: "30%", height: "2vw", border: 0 }}>
                    <StyledNewMappingButton onClick={handleNewMapping}>
                      <p> Add New Mapping</p>
                    </StyledNewMappingButton>
                  </StyledTd>
                )}
              </StyledTr>
            </StyledTable>
          </StyledRawTradeDiv>
        </StyledRawDiv>
        <StyledChildTableDiv>
          <StyledTable>
            <THead style={{ position: "sticky", top: "0px" }}>
              {table.getHeaderGroups().map((headerGroup) => (
                <StyledHeaderTR key={headerGroup.id}>
                  {headerGroup.headers.map((header) => {
                    return (
                      <StyledTh key={header.id} colSpan={header.colSpan}>
                        {header.isPlaceholder ? null : (
                          <>
                            {flexRender(
                              header.column.columnDef.header,
                              header.getContext()
                            )}
                          </>
                        )}
                      </StyledTh>
                    );
                  })}
                </StyledHeaderTR>
              ))}
            </THead>
            <TBody>
              {table.getRowModel()?.rows?.length > 0 ? (
                renderRows(table.getRowModel().rows)
              ) : (
                <StyledTR>
                  <StyledNoDataTd colSpan={10}>
                    No Mapping Details Found
                  </StyledNoDataTd>
                </StyledTR>
              )}
            </TBody>
          </StyledTable>
        </StyledChildTableDiv>
      </StyledDivChild>
    </>
  );
};
