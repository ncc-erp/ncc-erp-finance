import {
  Component,
  Inject,
  OnInit,
  ViewChild,
  ElementRef,
} from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { HttpClient } from "@angular/common/http";
import { BranchService } from "@app/service/api/branch.service";
import { BranchDto } from "@app/modules/branch/branch.component";
import { RequestDetailService } from "@app/service/api/request-detail.service";
import { catchError } from "rxjs/operators";

@Component({
  selector: "app-extract-detail",
  templateUrl: "./extract-detail.component.html",
  styleUrls: ["./extract-detail.component.css"],
})
export class ExtractDetailComponent implements OnInit {
  @ViewChild("fileInput") fileInput!: ElementRef;

  selectedFile: File | null = null;
  fileUrl: string = "";
  data: any;
  isLoading = false;
  isExtracted = false;
  inputMethod: "file" | "url" = "file"; // Default to file upload
  isDragOver = false;
  invalidFileType = false;

  // Allowed file types
  allowedFileTypes = [
    "application/pdf",
    "image/jpeg",
    "image/jpg",
    "image/png",
    "image/gif",
    "image/bmp",
  ];

  public branchList: BranchDto[] = [];

  constructor(
    private http: HttpClient,
    private branchService: BranchService,
    private requestDetailService: RequestDetailService,
    private dialogRef: MatDialogRef<ExtractDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {}

  ngOnInit(): void {
    this.getAllBranch();
  }

  getAllBranch() {
    this.branchService.GetAllForDropdown().subscribe((data) => {
      this.branchList = data.result;
    });
  }

  setInputMethod(method: "file" | "url") {
    this.inputMethod = method;
    // Clear the other input method data
    if (method === "file") {
      this.fileUrl = "";
    } else {
      this.selectedFile = null;
      this.invalidFileType = false;
    }
  }

  isValidFileType(file: File): boolean {
    // Check if file type is in allowed list
    return this.allowedFileTypes.includes(file.type);
  }

  isPdf(): boolean {
    return this.selectedFile?.type === "application/pdf";
  }

  isImage(): boolean {
    return this.selectedFile?.type.startsWith("image/");
  }

  selectFile(event: any) {
    const file = event.target.files?.[0] || null;
    if (file) {
      if (this.isValidFileType(file)) {
        this.selectedFile = file;
        this.invalidFileType = false;
      } else {
        this.selectedFile = file;
        this.invalidFileType = true;
        setTimeout(() => {
          // Reset the file input
          if (this.fileInput) {
            this.fileInput.nativeElement.value = "";
          }
        }, 100);
      }
      this.fileUrl = ""; // Clear URL when file is selected
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;

    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      const file = files[0];
      if (this.isValidFileType(file)) {
        this.selectedFile = file;
        this.invalidFileType = false;
      } else {
        this.selectedFile = file;
        this.invalidFileType = true;
        setTimeout(() => {
          // Reset the file input
          if (this.fileInput) {
            this.fileInput.nativeElement.value = "";
          }
        }, 100);
      }
      this.fileUrl = ""; // Clear URL when file is dropped
    }
  }

  canExtract(): boolean {
    return (
      (this.inputMethod === "file" &&
        this.selectedFile !== null &&
        !this.invalidFileType) ||
      (this.inputMethod === "url" && this.fileUrl.trim().length > 0)
    );
  }

  extract() {
    if (!this.canExtract()) {
      let message = "";
      if (this.inputMethod === "file") {
        if (this.invalidFileType) {
          message = "Vui lòng chọn file PDF hoặc ảnh để extract!";
        } else {
          message = "Vui lòng chọn file để extract!";
        }
      } else {
        message = "Vui lòng nhập URL file để extract!";
      }
      abp.message.warn(message);
      return;
    }

    this.isLoading = true;

    this.requestDetailService
      .extractFile(this.selectedFile, this.fileUrl)
      .subscribe(
        (res: any) => {
          this.isLoading = false;
          if (res?.success) {
            if (!res.data?.products || res.data.products.length === 0) {
              abp.message.warn(
                "Không thể nhận diện sản phẩm từ file. Vui lòng kiểm tra lại file hoặc thử file khác.",
                "Không có dữ liệu"
              );
              return;
            }

            this.data = res.data;
            this.data.products.forEach((item: any) => {
              item.branchId = this.branchList[0]?.id ?? null;
              item.paid = false;
            });
            this.isExtracted = true;
          } else {
            abp.message.error("Extract thất bại.");
          }
        },
        () => {
          this.isLoading = false;
          abp.message.error("Có lỗi xảy ra khi extract!");
        }
      );
  }

  removeRow(index: number) {
    this.data.products.splice(index, 1);
  }

  save() {
    const products = this.data?.products;

    if (!products || products.length === 0) {
      abp.message.warn("Không có dữ liệu để lưu.");
      return;
    }

    const invalid = products.filter(
      (item: any) =>
        !item.product_name ||
        !item.unit_quantity ||
        !item.unit_price ||
        !item.branchId
    );

    if (invalid.length > 0) {
      abp.message.warn("Vui lòng nhập đầy đủ thông tin trước khi lưu!");
      return;
    }

    const outcomingEntryId = this.dialogData.outcomingEntryId;

    const payload = products.map((p: any) => ({
      name: p.product_name,
      quantity: p.unit_quantity,
      unitPrice: p.unit_price,
      total: p.unit_quantity * p.unit_price,
      branchId: p.branchId,
      isNotDone: !p.paid,
      outcomingEntryId: outcomingEntryId,
    }));

    this.isLoading = true;

    this.requestDetailService
      .createMany(payload)
      .pipe(catchError(this.requestDetailService.handleError))
      .subscribe(
        (res) => {
          if (!!res?.body?.success || res?.body?.Success >= 0) {
            let message = `success <strong class='text-success'>${res?.body.result?.success}</strong>, fail <strong class='text-danger'>${res?.body.result?.fail}</strong>`;
            abp.message.info(message, "Import result", { isHTML: true });

            this.dialogRef.close(true);
          }
        },
        () => {
          this.isLoading = false;
        }
      );
  }

  resetExtraction() {
    this.isExtracted = false;
    this.selectedFile = null;
    this.fileUrl = "";
    this.data = null;
    this.invalidFileType = false;

    if (this.fileInput) {
      this.fileInput.nativeElement.value = "";
    }
    this.inputMethod = "file";
  }
}
